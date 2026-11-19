using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Eureka.Stroke.ReportService;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace EurekaTest
{
    /// <summary>
    /// Summary description for UnitTest2
    /// </summary>
    [TestClass]
    public class ReportTest
    {
        public ReportTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Initializer.BootStrap();
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void AddDeleteFolder()
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();

            var folders1 = reportService.GetFolders();

            var folderTest1 = reportService.AddFolder("test folder");
            Assert.IsNotNull(folderTest1);
            Assert.AreEqual(folders1.Count + 1, reportService.GetFolders().Count);

            var folderTest2 = reportService.GetFolderByID(folderTest1.ID);

            Assert.IsNotNull(folderTest2);
            Assert.AreSame(folderTest1, folderTest2);

            reportService.DeleteFolder(folderTest1.ID);

            Assert.AreEqual(folders1.Count, reportService.GetFolders().Count);
        }

        [TestMethod]
        public void UpdateFolder()
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();

            const string name = "new test folder";

            var folderTest1 = reportService.AddFolder("test folder");
            Assert.IsNotNull(folderTest1);

            reportService.UpdateFolder(folderTest1.ID, name);
            var folderTest2 = reportService.GetFolderByID(folderTest1.ID);
            Assert.IsNotNull(folderTest2);
            Assert.AreEqual(folderTest2.Name, name);

            reportService.DeleteFolder(folderTest1.ID);
        }

        [TestMethod]
        public void EnableFolder()
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();

            var folderTest1 = reportService.AddFolder("test folder");
            Assert.IsNotNull(folderTest1);
            Assert.AreEqual(folderTest1.Enabled, true);

            reportService.DisableFolder(folderTest1.ID);
            Assert.AreEqual(reportService.GetFolderByID(folderTest1.ID).Enabled, false);

            reportService.EnableFolder(folderTest1.ID);
            Assert.AreEqual(reportService.GetFolderByID(folderTest1.ID).Enabled, true);

            reportService.DeleteFolder(folderTest1.ID);
        }

        [TestMethod]
        public void AddDeleteReport()
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();

            var folderTest1 = reportService.AddFolder("test folder");
            var reports1 = reportService.GetReports();
            var report1 = reportService.AddReport(folderTest1.ID, "test report", "test report desc.");
            Assert.AreEqual(reports1.Count + 1, reportService.GetReports().Count);

            var report2 = reportService.GetReportByID(report1.ID);
            Assert.IsNotNull(report2);
            Assert.AreEqual(report2, report1);

            reportService.DeleteReport(report1.ID);
            Assert.AreEqual(reports1.Count, reportService.GetReports().Count);

            reportService.DeleteFolder(folderTest1.ID);
        }

        [TestMethod]
        public void UpdateReport()
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();

            var folderTest1 = reportService.AddFolder("test folder");
            var report1 = reportService.AddReport(folderTest1.ID, "test report", "test report desc.");

            var name = "new report name";
            var description = "new report description";
            reportService.UpdateReport(report1.ID, name, description);

            var report2 = reportService.GetReportByID(report1.ID);
            Assert.IsNotNull(report2);
            Assert.AreEqual(report2.Name, name);
            Assert.AreEqual(report2.Description, description);

            reportService.DeleteReport(report1.ID);
            reportService.DeleteFolder(folderTest1.ID);
        }

        [TestMethod]
        public void EnableReport()
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();

            var folderTest1 = reportService.AddFolder("test folder");
            var report1 = reportService.AddReport(folderTest1.ID, "test report", "test report desc.");

            reportService.DisableReport(report1.ID);
            Assert.IsFalse(reportService.GetReportByID(report1.ID).Enabled);

            reportService.EnableReport(report1.ID);
            Assert.IsTrue(reportService.GetReportByID(report1.ID).Enabled);

            reportService.DeleteReport(report1.ID);
            reportService.DeleteFolder(folderTest1.ID);
        }

        [TestMethod]
        public void UserFolders()
        {
            var userID = 1;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            var folderTest1 = reportService.AddFolder("test folder");

            var nObjects = reportService.GetUserObjects(userID).Count;
            var nUserFolders = reportService.GetFoldersByUser(userID).Count;

            reportService.UserGrantFolder(userID, folderTest1.ID);
            Assert.AreEqual(nObjects + 1, reportService.GetUserObjects(userID).Count);

            Assert.AreEqual(nUserFolders + 1, reportService.GetFoldersByUser(userID).Count);
            Assert.AreEqual(0, reportService.GetFoldersByUser(2).Count);

            reportService.UserRevokeFolder(userID, folderTest1.ID);
            Assert.AreEqual(nObjects, reportService.GetUserObjects(userID).Count);

            reportService.DeleteFolder(folderTest1.ID);
        }

        [TestMethod]
        public void UserReports()
        {
            var userID = 1;
            var reportService = new Eureka.Stroke.ReportService.ReportService();

            var folderTest1 = reportService.AddFolder("test folder");
            var report1 = reportService.AddReport(folderTest1.ID, "test report", "test report desc.");

            var nObjects = reportService.GetUserObjects(userID).Count;
            var nUserReports = reportService.GetReportsByUser(userID).Count;
            var nUserFolderReports = reportService.GetFolderReportsByUser(folderTest1.ID, userID).Count;

            reportService.UserGrantReport(userID, report1.ID);
            Assert.AreEqual(nObjects + 1, reportService.GetUserObjects(userID).Count);

            Assert.AreEqual(nUserReports + 1, reportService.GetFolderReportsByUser(folderTest1.ID, userID).Count);
            Assert.AreEqual(0, reportService.GetFolderReportsByUser(folderTest1.ID, 2).Count);

            Assert.AreEqual(nUserFolderReports + 1, reportService.GetReportsByUser(userID).Count);
            Assert.AreEqual(0, reportService.GetReportsByUser(2).Count);

            reportService.UserRevokeReport(userID, report1.ID);
            Assert.AreEqual(nObjects, reportService.GetUserObjects(userID).Count);

            reportService.DeleteReport(report1.ID);
            reportService.DeleteFolder(folderTest1.ID);
        }




    }
}
