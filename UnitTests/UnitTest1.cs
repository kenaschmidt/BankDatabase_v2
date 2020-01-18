using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankDatabase_v2;
using BankDatabase_v2.DataModel;
using BankDatabase_v2.Extensions;
using BankDatabase_v2.DataControl;
using System.Threading;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using Microsoft.SqlServer.FFIECWebService;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        BankDatabase database = new BankDatabase();

        FFIEC_Interface WebdataInterface;

        [TestMethod]
        public void TestWebservice()
        {

            WebdataInterface = new FFIEC_Interface(database);
            
        }

        [TestMethod]
        public void TestDatabase()
        {

            using (var db = database.GetContext())
            {

                Console.WriteLine("Banks: {0}", db.Banks.Count());
                Console.WriteLine("Reports: {0}", db.Reports.Count());
                Console.WriteLine("Points: {0}", db.DataPoints.Count());


            }

        }
            

    }
}
