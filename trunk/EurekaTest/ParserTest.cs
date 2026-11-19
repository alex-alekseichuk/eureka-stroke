using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke;
using Eureka.Stroke.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using System.Data.Entity;
using Eureka.Stroke.Data;
using System.Configuration;
namespace EurekaTest
{
    [TestClass]
    public class ParserTests
    {
        public ParserTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;
        //private static readonly IUnityContainer container = new UnityContainer();

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

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            //container = new UnityContainer();
            Initializer.BootStrap();

        }
        #region Parse

        [TestMethod]
        public void TestParse()
        {
            ParseInsert();
            ParseUpdate();
        }

        private void ParseInsert()
        {
            EurekaStrokeEntities ctx = Initializer.DIContainer.Resolve<EurekaStrokeEntities>();
            Format fmt = DataFacade.GetAlienFormat(ctx);
            Facility facility = DataFacade.GetFacilityByFormat(ctx, fmt);

            UploadLog log = new UploadLog()
            {
         
                //Filename = "C:\\Projects\\Rovbut\\Test\\files\\testBusinessRuleDates6.csv",
                Filename = ConfigurationManager.AppSettings["TestFiles"] + "\\outcome_1error.csv",
                OriginalFilename = "outcome_1error.csv",
                Facility = facility, // TODO: initilaize facility
                ErrorCount = 0,
                TotalCount = 0,
                ChangesCount = 0,
                StartedBy = null,
                StartedDate = DateTime.Now

            };

            IParser parser =Initializer.DIContainer.Resolve<IParser>();
            Assert.IsNotNull(parser, "Parser null");
            Result res = parser.Parse(Mode.Offline, log);

            Assert.IsTrue(log.ErrorCount == 2);
            Assert.IsTrue(log.StrokeData.Count == 5);
            Assert.IsTrue(log.StrokeData.First().StrokeFieldValue.Count > 0,"Values not populated");
        }

         private void ParseUpdate()
        {
            EurekaStrokeEntities ctx = Initializer.DIContainer.Resolve<EurekaStrokeEntities>();
            Format fmt = DataFacade.GetAlienFormat(ctx);
            Facility facility = DataFacade.GetFacilityByFormat(ctx, fmt);

            UploadLog log = new UploadLog()
            {
         
                //Filename = "C:\\Projects\\Rovbut\\Test\\files\\testBusinessRuleDates6.csv",
                Filename = ConfigurationManager.AppSettings["TestFiles"] + "\\outcome_2Changes1error.csv",
                OriginalFilename = "outcome_2Changes1error.csv",
                Facility = facility, // TODO: initilaize facility
                ErrorCount = 0,
                TotalCount = 0,
                ChangesCount = 0,
                StartedBy = null,
                StartedDate = DateTime.Now

            };

            IParser parser =Initializer.DIContainer.Resolve<IParser>();
            Assert.IsNotNull(parser, "Parser null");
            Result res = parser.Parse(Mode.Offline, log);

            Assert.IsTrue(log.ErrorCount == 2);
            Assert.IsTrue(log.ChangesCount == 2);
            Assert.IsTrue(log.CorrectionsCount == 3);
            Assert.IsTrue(log.StrokeData.Count == 5);
        }
        

        #endregion Parse
    }
}
