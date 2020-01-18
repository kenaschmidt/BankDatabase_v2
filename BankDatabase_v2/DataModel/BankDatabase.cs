using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BankDatabase_v2.Extensions;

namespace BankDatabase_v2.DataModel
{
    public class BankDatabase
    {

        public BankDatabase()
        {

        }

        /// <summary>
        /// Return a new Database Context
        /// </summary>
        /// <returns></returns>
        public BankDatabaseContext GetContext()
        {
            return new BankDatabaseContext();
        }

    }

    public partial class Bank : IEqualityComparer<Bank>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RSSD { get; set; }

        public string BankName { get; set; }
        public string BankCity { get; set; }
        public string BankAddress { get; set; }
        public string BankState { get; set; }
        public int BankZip { get; set; }

        public virtual List<Report> Reports { get; set; }

        #region Methods

        /// <summary>
        /// Generic Constructor
        /// </summary>
        public Bank()
        {
            Reports = new List<Report>();
        }

        /// <summary>
        /// Returns existing report with matching report date, or if none exists, creates and returns a new rport.
        /// </summary>
        /// <param name="ReportDate"></param>
        /// <returns></returns>
        public Report GetReport(DateTime ReportDate)
        {
            return (Reports.Find(x => x.ReportDate == ReportDate) ??
                Reports.AddAndReturn<Report>(new Report { ReportDate = ReportDate, Bank = this, DataPoints = new List<DataPoint>() }));
        }

        public bool Equals(Bank x, Bank y)
        {
            return x.RSSD.Equals(y.RSSD) ? true : false;
        }

        public int GetHashCode(Bank obj)
        {
            return obj.RSSD;
        }

        #endregion
    }

    public partial class Report : IEqualityComparer<Report>
    {
        [Key]
        public int ReportId { get; set; }

        public DateTime ReportDate { get; set; }

        public virtual List<DataPoint> DataPoints { get; set; }
        public virtual Bank Bank { get; set; }

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_ReportDate"></param>
        public Report() { }

        /*
        public void SetValue(DataPoint _DataPoint, bool ForceReplace = false)
        {
            if (!ForceReplace)
                DataPoints.AddOrReplace(_DataPoint);
            else
                DataPoints.Add(_DataPoint);
        }
        */

        public void SetValue(string _FieldCode, decimal _Value)
        {
            DataPoints.AddOrReplace(new DataPoint { FieldCode = _FieldCode, Value = _Value, Report = this, Bank = Bank });
        }

        public DataPoint GetValue(string _FieldCode)
        {
            return (from dp in DataPoints
                    where dp.FieldCode == _FieldCode
                    select dp).FirstOrDefault() ?? new DataPoint { FieldCode = _FieldCode, Value = 0 };
        }

        public bool Equals(Report x, Report y)
        {
            return (x.Bank.Equals(y.Bank) && x.ReportDate.Equals(y.ReportDate)) ? true : false;
        }

        public int GetHashCode(Report obj)
        {
            return ReportId * (ReportDate.GetHashCode());
        }

        #endregion

    }

    public partial class DataPoint : IEqualityComparer<DataPoint>, IEquatable<DataPoint>
    {
        [Key]
        public int DataPointId { get; set; }

        public string FieldCode { get; set; }

        public decimal Value { get; set; }

        public virtual Report Report { get; set; }
        public virtual Bank Bank { get; set; }

        #region Methods

        public DataPoint() { }

        public bool Equals(DataPoint x, DataPoint y)
        {
            return (x.FieldCode == y.FieldCode ? true : false);
        }

        public int GetHashCode(DataPoint obj)
        {
            return FieldCode.GetHashCode();
        }

        public bool Equals(DataPoint other)
        {
            if (FieldCode == other.FieldCode)
                return true;

            return false;

        }

        #endregion
    }

    public partial class Description
    {
        [Key]
        public string FieldCode { get; set; }

        public string Text { get; set; }

        public bool FilterVisible { get; set; }

        #region Methods

        public Description() { }

        #endregion
    }

    public class BankDatabaseContext : DbContext
    {

        public DbSet<Bank> Banks { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<DataPoint> DataPoints { get; set; }
        public DbSet<Description> Descriptions { get; set; }

        public BankDatabaseContext(bool DropCreate = false)
        {
            if (DropCreate)
                Database.SetInitializer(new DropCreateDatabaseAlways<BankDatabaseContext>());

            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<BankDatabaseContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataPoint>().Property(x => x.Value).HasPrecision(16, 4);
        }

        /// <summary>
        /// Returns existing bank, or creates a new bank and returns
        /// </summary>
        /// <param name="RSSD"></param>
        /// <returns></returns>
        public Bank GetBank(int _RSSD)
        {
            Bank bank = (from b in Banks
                         where b.RSSD == _RSSD
                         select b).FirstOrDefault() ?? Banks.AddAndReturn<Bank>(new Bank { RSSD = _RSSD });
            return bank;
        }

        /// <summary>
        /// Returns a list of banks with valid reports for a specified period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public List<Bank> GetBanksByPeriod(DateTime Period)
        {
            return (from report in Reports where report.ReportDate == Period select report.Bank).ToList();
        }

        /// <summary>
        /// Returns a list of banks with valid reports for a specified period without any currently valid report data
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public List<Bank> GetBanksByPeriodWithoutDate(DateTime Period)
        {
            return (from report in Reports where (report.ReportDate == Period && report.DataPoints.Count == 0) select report.Bank).ToList();
        }

    }

}
