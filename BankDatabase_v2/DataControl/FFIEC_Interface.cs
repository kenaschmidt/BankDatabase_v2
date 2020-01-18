using Microsoft.Web.Services3.Security.Tokens;
using Microsoft.SqlServer.FFIECWebService;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using BankDatabase_v2.DataModel;
using BankDatabase_v2.Extensions;
using JeffFerguson.Gepsio;
using System.IO;
using System.Linq;
using System.Threading;

namespace BankDatabase_v2.DataControl
{
    /// <summary>
    /// Handles all interaction with the FFIEC webservice
    /// </summary>
    public class FFIEC_Interface
    {

        // FFIEC connection parameters
        private const string WebToken = "hQJ1sMxSIxWrZTAEvt1T";
        private const string WebUsername = "kenaschmidt";

        private string _TemporaryDirectory = "C:\\Users\\kschmidt\\Desktop\\XBRLTempFolder\\";
        public string TemporaryDirectory
        {
            get
            {
                return _TemporaryDirectory;
            }
            set
            {
                _TemporaryDirectory = value;
            }
        }

        UsernameToken userToken = new UsernameToken(WebUsername, WebToken, PasswordOption.SendHashed);

        // FFIEC interface
        RetrievalService proxy;
        public bool Connected { get; set; } = false;

        // Database interface
        BankDatabase database;

        // List of all FFIEC valid reporting periods for which we can request data
        // Loaded upon connection
        public List<DateTime> AllReportingPeriods { get; set; } = new List<DateTime>();

        #region (EVENTS)

        /// <summary>
        /// Sends a true or false indicating current object status
        /// </summary>
        public event EventHandler<FlagEventArgs> Ready;
        protected virtual void OnReady(bool SetFlag)
        {
            Ready?.Invoke(this, new FlagEventArgs { Flag = SetFlag });
        }

        public event EventHandler<EventArgs> DataRequestComplete;
        protected virtual void OnDataRequestComplete()
        {
            DataRequestComplete?.Invoke(this, new EventArgs());
        }

        public event EventHandler<CountEventArgs> RequestsRemaining;
        protected virtual void OnRequestsRemaining(int Value)
        {
            RequestsRemaining?.Invoke(this, new CountEventArgs { Count = Value });
        }

        #endregion

        #region (METHODS) Constructors and Initializers

        /// <summary>
        /// Constructor
        /// </summary>
        public FFIEC_Interface(BankDatabase database)
        {
            this.database = database;
        }

        /// <summary>
        /// Initialize webservice connection and local controls
        /// </summary>
        public void InitializeServices()
        {
            // Initialize a connection with the webservice
            // If successful, initialize this object/handlers
            try
            {
                proxy = new RetrievalService();

#pragma warning disable CS0618 // Type or member is obsolete
                proxy.RequestSoapContext.Security.Tokens.Add(userToken);
#pragma warning restore CS0618 // Type or member is obsolete

                // Connection Success
                if (proxy.TestUserAccess())
                {
                    Console.WriteLine("FFIEC Connected :: User Authorized");

                    Connected = true;
                    // Set up async handlers
                    InitializeWebserviceHandlers();

                    // Request valid reporting periods
                    RequestValidReportingPeriods();

                }
                // Connection failure
                else
                    Console.WriteLine("Failed to authorize FFIED User");

            }
            catch (Exception) { Console.WriteLine("Webservice Connection Error"); }
        }

        /// <summary>
        /// Attaches handlers for webservice data receipt
        /// </summary>
        private void InitializeWebserviceHandlers()
        {

            // Handles valid reporting date request
            proxy.RetrieveUBPRReportingPeriodsCompleted += Proxy_RetrieveUBPRReportingPeriodsCompleted;

            // Handles panel of reporters request
            proxy.RetrievePanelOfReportersCompleted += Proxy_RetrievePanelOfReportersCompleted;

            // Handles XBRL report requests
            proxy.RetrieveUBPRXBRLFacsimileCompleted += Proxy_RetrieveUBPRXBRLFacsimileCompleted;

        }

        #endregion

        #region (METHODS) Valid Reporting Periods

        /// <summary>
        /// Initiates an Async request with the webservice
        /// </summary>
        private void RequestValidReportingPeriods()
        {
            // Send async request for a list of valid reporting periods
            proxy.RetrieveUBPRReportingPeriodsAsync();
        }

        /// <summary>
        /// Receives Proxy callback for requesting a list of FFIEC valid reporting periods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Proxy_RetrieveUBPRReportingPeriodsCompleted(object sender, RetrieveUBPRReportingPeriodsCompletedEventArgs e)
        {
            try
            {
                foreach (string period in e.Result)
                {
                    DateTime.TryParse(period, out DateTime dt);
                    AllReportingPeriods.Add(dt);
                }

                AllReportingPeriods.Reverse();
                OnReady(true);
            }
            catch (Exception) { Console.WriteLine("Error storing reporting period value"); }
        }

        #endregion

        #region (METHODS) Valid Reporting Banks

        /// <summary>
        /// Submits requests to retrieve reporting RSSDs for each period in ValidReportingPeriods
        /// Waits between request processes to maintain 
        /// </summary>
        public void LoadReportingInstitutions(DateTime Period)
        {
            proxy.RetrievePanelOfReportersAsync(ReportingDataSeriesName.Call, Period.ToString(), Period);
        }

        private Queue<RetrievePanelOfReportersCompletedEventArgs> PanelQueue = new Queue<RetrievePanelOfReportersCompletedEventArgs>();
        private bool PanelQueueWorking = false;

        /// <summary>
        /// Callback for all reporting banks in specified period
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Proxy_RetrievePanelOfReportersCompleted(object sender, RetrievePanelOfReportersCompletedEventArgs e)
        {
            PanelQueue.Enqueue(e);

            if (!PanelQueueWorking)
            {
                PanelQueueWorking = true;
                new Thread(() =>
                {
                    while (PanelQueue.Count > 0)
                    {
                        RetrievePanelOfReportersCompletedEventArgs ee = PanelQueue.Dequeue();

                        //For each bank, create a new entry if none exists, and add identifying information
                        Bank newBank;
                        int pendingRequests = ee.Result.Count();

                        OnRequestsRemaining(pendingRequests);

                        using (var db = database.GetContext())
                        {

                            // Each result will have a panel of bank objects to parse
                            foreach (ReportingFinancialInstitution bank in ee.Result)
                            {
                                // Get or create the bank in our database
                                newBank = db.GetBank(bank.ID_RSSD);

                                // Update fields
                                newBank.BankName = bank.Name;
                                newBank.BankState = bank.State;
                                newBank.BankCity = bank.City;
                                newBank.BankAddress = bank.Address;
                                newBank.BankZip = bank.ZIP;

                                // Set report shell for this period
                                newBank.GetReport((DateTime)ee.UserState);

                                OnRequestsRemaining(--pendingRequests);
                            }

                            // Save changes
                            db.SaveChanges();
                            OnRequestsRemaining(0);

                        }

                    }
                    PanelQueueWorking = false;
                }).Start();
            }
        }

        #endregion

        #region (METHODS) XBRL Retrieval

        private int ActiveReportRequests = 0;

        private Queue<RetrieveUBPRXBRLFacsimileCompletedEventArgs> ReportQueue = new Queue<RetrieveUBPRXBRLFacsimileCompletedEventArgs>();
        private bool ReportQueueWorking = false;

        /// <summary>
        /// Function to submit request for an XBRL report for one bank, one period
        /// Results are stored in database by retrieval function
        /// </summary>
        /// <param name="Bank"></param>
        /// <param name="Period"></param>
        public void RequestXBRLReport(Bank Bank, DateTime Period)
        {
            try
            {
                proxy.RetrieveUBPRXBRLFacsimileAsync(Period.ToString(), FinancialInstitutionIDType.ID_RSSD, Bank.RSSD, new object[] { Period, Bank.RSSD });
                OnRequestsRemaining(++ActiveReportRequests);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not submit request for XBRL: ID {0} DATE {1}", Bank.RSSD, Period);
            }
        }

        /// <summary>
        /// Receives completed requests for XBRL reports
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Proxy_RetrieveUBPRXBRLFacsimileCompleted(object sender, RetrieveUBPRXBRLFacsimileCompletedEventArgs e)
        {

            ReportQueue.Enqueue(e);

            if (!ReportQueueWorking)
            {
                ReportQueueWorking = true;
                new Thread(() =>
                {
                    while (ReportQueue.Count > 0)
                    {
                        RetrieveUBPRXBRLFacsimileCompletedEventArgs ee = ReportQueue.Dequeue();

                        int pendingRequests = ReportQueue.Count();
                        OnRequestsRemaining(pendingRequests);

                        // Convert the user-passed data back into an array
                        object[] userState = (object[])ee.UserState;
                        int RSSD = (int)userState[1];
                        DateTime Period = (DateTime)userState[0];

                        //e.Result contains a byte array that we need to save as an XML file to process with Gepsio
                        ProcessXBRLdata(ee.Result, RSSD, Period);
                    }
                    ReportQueueWorking = false;
                }).Start();
            }             


          
        }

        /// <summary>
        /// Calls functios needed to process and store XBRL file
        /// </summary>
        /// <param name="DataByteArray"></param>
        /// <param name="RSSD"></param>
        /// <param name="Period"></param>
        private void ProcessXBRLdata(byte[] DataByteArray, int RSSD, DateTime Period)
        {
            string TempFile = CreateTemporaryFile(DataByteArray, RSSD, Period);
            if (SaveXBRLtoDatabase(TempFile, RSSD, Period))
            {
                DeleteTemporaryFile(TempFile);
                OnRequestsRemaining(--ActiveReportRequests);
            }
        }

        /// <summary>
        /// Writes XBRL data to a temporary file and returns the handle
        /// </summary>
        /// <param name="ByteArray"></param>
        /// <param name="RSSD"></param>
        /// <param name="Period"></param>
        /// <returns></returns>
        private string CreateTemporaryFile(byte[] ByteArray, int RSSD, DateTime Period)
        {
            try
            {
                // Get file string token
                string TempFileName = GetReportFilename(RSSD, Period);

                // Create filestream
                FileStream fileStream = new FileStream(TemporaryDirectory + TempFileName, FileMode.Create, FileAccess.ReadWrite);
                fileStream.Write(ByteArray, 0, ByteArray.Length);
                fileStream.Close();
                fileStream.Dispose();

                return (TemporaryDirectory + TempFileName);
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Opens a XML file in the Gepsio framework and stores all field values to the database
        /// </summary>
        /// <param name="TemporaryFilePath"></param>
        /// <returns></returns>
        private bool SaveXBRLtoDatabase(string TemporaryFilePath, int RSSD, DateTime Period)
        {
            // Make sure we were passed a valid file
            if (!File.Exists(TemporaryFilePath))
                throw new FileNotFoundException();

            XbrlDocument LocalReport = new XbrlDocument();

            try
            {
                // Checks to see if a file exists.  If it does, load the file and return the XBLR document, otherwise return null
                LocalReport.Load(TemporaryFilePath);
            }
            catch (Exception) { Console.WriteLine("Could not load XbrlDocument from Temporary File"); return false; }

            try
            {
                using (var db = database.GetContext())
                {
                    // Get the report shell from the database
                    Report DatabaseReport = db.GetBank(RSSD).GetReport(Period);

                    // Each report may contain the current period value, as well as prior quarter and prior year values, identified in the 'contextrefname'
                    // Filter out values from prior periods and store the valid items in an array of Item objects
                    Item[] Facts = (from Item f in LocalReport.XbrlFragments[0].Facts.AsParallel()
                                    where f.ContextRefName.Contains(Period.ToString("yyyy-MM-dd"))
                                    select f).ToArray();

                    // Parse each Item and send to the database for storage
                    for (int i = 0; i < Facts.Count(); i++)
                    {
                        if (Decimal.TryParse(Facts[i].Value, out decimal parseResultDec))
                        {
                            DatabaseReport.SetValue(Facts[i].Name, parseResultDec);
                        }
                        else if (Boolean.TryParse(Facts[i].Value, out Boolean parseResultBool))
                        {
                            DatabaseReport.SetValue(Facts[i].Name, Convert.ToDecimal(parseResultBool));
                        }
                        else
                        {
                            DatabaseReport.SetValue(Facts[i].Name, 0);
                        }
                    }

                    db.SaveChanges();

                    Console.WriteLine("Saved XBRL data to database for ID: {0}  Date: {1}", RSSD, Period);

                    LocalReport = null;
                    Facts = null;

                    return true;
                }
            }
            catch (Exception ex) { Console.WriteLine("Report load error: {0}", ex.Message); }
            finally
            {
                LocalReport = null;
            }

            return false;
        }

        /// <summary>
        /// Deletes the temporary file after use
        /// </summary>
        /// <param name="TemporaryFilePath"></param>
        /// <returns></returns>
        private bool DeleteTemporaryFile(string TemporaryFilePath)
        {
            // Make sure we are only deleting files from the temporary file path
            if (!TemporaryFilePath.Contains(TemporaryDirectory))
                return false;

            if (!TemporaryFilePath.Contains("temp"))
                return false;

            File.Delete(TemporaryFilePath);

            return true;
        }

        /// <summary>
        /// Returns a formatted string name of an XBRL report temp file
        /// </summary>
        /// <param name="RSSD"></param>
        /// <param name="ReportDate"></param>
        /// <returns></returns>
        public string GetReportFilename(int RSSD, DateTime ReportDate)
        {
            return (string.Format("{0}_{1}_temp.xml", RSSD, ReportDate.ToString("yyyyMMdd")));
        }


        #endregion


    }
}
