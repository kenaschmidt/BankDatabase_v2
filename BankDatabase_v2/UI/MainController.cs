using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BankDatabase_v2.DataControl;
using BankDatabase_v2.DataModel;
using BankDatabase_v2.Extensions;

namespace BankDatabase_v2.UI
{
    public partial class MainController : Form
    {

        BankDatabase database = new BankDatabase();
        FFIEC_Interface webdata;

        #region Initialization

        public MainController()
        {
            InitializeComponent();

            InitializeWebController();

            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set status box
            textBoxWebDataStatus.DeselectAll();
            textBoxWebDataStatus.TabStop = false;
            textBoxWebDataStatus.ReadOnly = true;
            textBoxWebDataStatus.GotFocus += (s, e) => { this.Focus(); };

            // Status scroller
            webdata.RequestsRemaining += Webdata_RequestsRemaining;

            // Set toolbar -> Data -> XBRL Path
            selectXBRLPathToolStripMenuItem.Click += (s, e) =>
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    webdata.TemporaryDirectory = dialog.SelectedPath + "\\";
                }
            };

            // Set toolbar -> System -> Initialize
            initializeToolStripMenuItem.Click += (s, e) =>
             {
                 if (!webdata.Connected)
                 {
                     webdata.InitializeServices();
                 }
             };

            // Set toolbar -> Data -> Test Load Banks
            LoadBanksToolStripMenuItem.Click += (s, e) =>
            {
                foreach (DateTime Period in webdata.AllReportingPeriods)
                    webdata.LoadReportingInstitutions(Period);
            };

            // Testing
            downloadXBRLReportsToolStripMenuItem.Click += DownloadXBRLReportsToolStripMenuItem_Click;

            // Report Period Box
            webdata.Ready += (s, f) =>
            {
                foreach (DateTime period in webdata.AllReportingPeriods)
                    checkedListBox_ReportingPeriods.Items.Add(period);
                checkedListBox_ReportingPeriods.Refresh();
                button_StartReportDownload.Enabled = true;
            };

            // Download reports button
            button_StartReportDownload.Click += (s, e) =>
            {

                using (var db = database.GetContext())
                {
                    foreach (DateTime selectedPeriod in checkedListBox_ReportingPeriods.SelectedItems)
                        foreach(Bank bank in db.GetBanksByPeriodWithoutDate(selectedPeriod))
                        {
                            webdata.RequestXBRLReport(bank, selectedPeriod);
                        }

                }
            };

            // Test print statistics
            printStatisticsToolStripMenuItem.Click += (s, e) =>
            {
                using (var db = database.GetContext())
                {
                    Console.WriteLine("Banks: {0}", db.Banks.Count());
                    Console.WriteLine("Reports: {0}", db.Reports.Count());
                    Console.WriteLine("Points: {0}", db.DataPoints.Count());

                    foreach(DateTime period in webdata.AllReportingPeriods)
                    {
                        Console.WriteLine("Period {0}  Reports {1}", period, db.GetBanksByPeriod(period).Count);
                    }

                }
            };
        }

        private void InitializeWebController()
        {

            webdata = new FFIEC_Interface(database);

            // Webdata events
            webdata.Ready += Webdata_Ready;
            webdata.DataRequestComplete += (s, e) =>
            {
                MessageBox.Show("Data Request Complete");
            };

        }

        #endregion

        #region Form Controls

        /// <summary>
        /// Event handler responds to Read/Not Ready events from the webdata conmt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Webdata_Ready(object sender, FlagEventArgs e)
        {
            switch (e.Flag)
            {
                case true:
                    if (!webdata.Connected)
                        return;
                    textBoxWebDataStatus.Text = "READY";
                    textBoxWebDataStatus.BackColor = Color.Green;
                    initializeToolStripMenuItem.Enabled = false;
                    break;
                case false:
                    if (webdata.Connected)
                        return;
                    textBoxWebDataStatus.Text = "NOT READY";
                    textBoxWebDataStatus.BackColor = Color.Red;
                    initializeToolStripMenuItem.Enabled = true;
                    break;
            }
        }

        delegate void Webdata_RequestsRemainingThreadsafe(object sender, CountEventArgs e);
        /// <summary>
        /// Updates scroll bar and status box when the webdata controller is processing requests
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Webdata_RequestsRemaining(object sender, CountEventArgs e)
        {
            if (InvokeRequired)
            {
                Webdata_RequestsRemainingThreadsafe del = new Webdata_RequestsRemainingThreadsafe(Webdata_RequestsRemaining);
                Invoke(del, new object[] { sender, e });
                return;
            }

            if (e.Count == 0)
            {
                progressbarWebData.Style = ProgressBarStyle.Blocks;
                progressbarWebData.MarqueeAnimationSpeed = 0;
                Webdata_Ready(null, new FlagEventArgs { Flag = true });
            }
            else
            {
                if (progressbarWebData.Style != ProgressBarStyle.Marquee)
                {
                    progressbarWebData.Style = ProgressBarStyle.Marquee;
                    progressbarWebData.MarqueeAnimationSpeed = 50;
                    textBoxWebDataStatus.BackColor = Color.Orange;
                }
                textBoxWebDataStatus.Text = string.Format("WORKING: {0}", e.Count);
            }
        }

        /// <summary>
        /// Load a select set of XBRL reports to test functionality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadXBRLReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var db = database.GetContext())
            {
                List<Bank> banks = db.Banks.ToList();

                for (int i = 10; i < 20; i++)
                {
                    webdata.RequestXBRLReport(banks[i], banks[i].Reports.FirstOrDefault().ReportDate);
                }



            }
        }


        #endregion

    }
}
