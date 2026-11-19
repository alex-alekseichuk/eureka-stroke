using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Eureka.Stroke.FormatService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eureka.Stroke;
using Eureka.Stroke.Converter;
using Eureka.Stroke.Data;
using System.Diagnostics;

namespace EurekaTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
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
            //container = new UnityContainer();
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
        public void TestMethod1()
        {
            //
            // TODO: Add test logic	here
            //
        }

        [TestMethod]
        public void PopulateData()
        {
            DataDictionary d = new DataDictionary();
            d.Populate();
        }

        [TestMethod]
        public void PopulateFakedData()
        {
            DataDictionary d = new DataDictionary();
            d.PopulateFaked();
        }

        [TestMethod]
        public void TestDeleteField()
        {
            DataDictionary d = new DataDictionary();
            d.PopulateFaked();

            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            var formatService = Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;

            var f = DataFacade.GetAlienFieldDefinition(ctx, "test", fmtAlien);
            string msg;
            int ok = DataFacade.TryDeleteAlienFieldDefinition(ctx, f.ID, out msg);

            Debug.Print(msg);
            Assert.IsTrue(ok == 0);

            var f2 = DataFacade.GetEurekaFieldDefinition(ctx, "TestNative", fmtEureka);
            ok = DataFacade.TryDeleteEurekaFieldDefinition(ctx, f2.ID, out msg);
            Debug.Print(msg);
            Assert.IsTrue(ok == 0);

        }

        [TestMethod]
        public void TestDeleteField2()
        {
            DataDictionary d = new DataDictionary();
            d.PopulateFaked();

            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            var formatService = Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;

            string msg;

            var f2 = DataFacade.GetEurekaFieldDefinition(ctx, "TestNative", fmtEureka);
            int ok = DataFacade.TryDeleteEurekaFieldDefinition(ctx, f2.ID, out msg);
            Debug.Print(msg);
            Assert.IsTrue(ok == 0);

            //var f = DataFacade.GetFieldDefinition(ctx, "test", fmtAlien);
            //ok = DataFacade.TryDeleteFieldDefinition(ctx, f.ID, out msg);

            //Debug.Print(msg);
            //Assert.IsTrue(ok);

            //ok = DataFacade.TryDeleteFieldDefinition(ctx, f2.ID, out msg);
            //Debug.Print(msg);
            //Assert.IsTrue(ok);

            

        }

        [TestMethod]
        public void TestSingleSelect()
        {
            int inputValue = 1;

            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            IValueConverter converter = Container.DIContainer.Resolve<IValueConverter>(EnumFieldType.SingleSelect.ToString());
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            var formatService = Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;

            AlienField source = DataFacade.GetAlienFieldDefinition(ctx, "jc_clinical", fmtAlien);
            EurekaField target = source.EurekaField;

            Assert.IsNotNull(target);

            var values = converter.Convert(source, target, inputValue.ToString());

            Assert.IsNotNull(values);
            Assert.IsTrue(values.Count() > 0);

            var val = values.First();

            int intVal = val.IntegerValue.Value;
            Assert.IsTrue(intVal == inputValue);

            Assert.IsTrue(converter.ErrorCode == 0);

        }

        // gs_symptomlocation
        [TestMethod]
        public void TestSingleSelect2()
        {
            int inputValue = 5;

            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            var formatService = Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;

            AlienField source = DataFacade.GetAlienFieldDefinition(ctx, "gs_symptomlocation", fmtAlien);
            EurekaField target = source.EurekaField;

            EnumFieldType type = (EnumFieldType)target.FieldType;
            IValueConverter converter = Container.DIContainer.Resolve<IValueConverter>(type.ToString());
                        

            Assert.IsNotNull(target);

            var values = converter.Convert(source, target, inputValue.ToString());

            Assert.IsNotNull(values);
            Assert.IsTrue(values.Count() > 0);

            var val = values.First();

            int intVal = val.IntegerValue.Value;
            //Assert.IsTrue(intVal == 4);

            Assert.IsTrue(converter.ErrorCode == 0);

        }


        [TestMethod]
        public void TestMultipleSelect()
        {
            string inputValue = "1:3";

            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            var formatService = Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;

            AlienField source = DataFacade.GetAlienFieldDefinition(ctx, "gs_race", fmtAlien);
            EurekaField target = source.EurekaField;
            
            EnumFieldType type = (EnumFieldType)target.FieldType;
            IValueConverter converter = Container.DIContainer.Resolve<IValueConverter>(type.ToString());

            
            Assert.IsNotNull(target);

            var values = converter.Convert(source, target, inputValue.ToString());

            Assert.IsNotNull(values);
            Assert.IsTrue(values.Count() > 0);

            var val1 = values.First();

            int intVal = val1.IntegerValue.Value;
            Assert.IsTrue(intVal == 1);

            var val2 = (from v in values select v).Skip(1).First();
            intVal = val2.IntegerValue.Value;
            Assert.IsTrue(intVal == 3);

            Assert.IsTrue(converter.ErrorCode == 0);

        }

        [TestMethod]
        public void TestDateTime()
        {
            string inputValue = "12/04/2009";
            DateTime dt = new DateTime(2009, 12, 4);

            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            IValueConverter converter = Container.DIContainer.Resolve<IValueConverter>(EnumFieldType.DateTime.ToString());
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            var formatService = Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;

            AlienField source = DataFacade.GetAlienFieldDefinition(ctx, "jc_admitdate", fmtAlien);
            EurekaField target = source.EurekaField;

            Assert.IsNotNull(target);

            var values = converter.Convert(source, target, inputValue.ToString());

            Assert.IsNotNull(values);
            Assert.IsTrue(values.Count() > 0);

            var val = values.First();

            DateTime dVal = val.DateTimeValue.Value;
            Assert.IsTrue(dVal == dt);

            Assert.IsTrue(converter.ErrorCode == 0);

        }

        [TestMethod]
        public void TestDecimal()
        {
            string inputValue = "456.";
           
            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            IValueConverter converter = Container.DIContainer.Resolve<IValueConverter>(EnumFieldType.Decimal.ToString());
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            var formatService = Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;

            AlienField source = DataFacade.GetAlienFieldDefinition(ctx, "gs_hba1c", fmtAlien);
            EurekaField target = source.EurekaField;

            Assert.IsNotNull(target);

            var values = converter.Convert(source, target, inputValue);

            Assert.IsNotNull(values);
            Assert.IsTrue(values.Count() > 0);

            var val = values.First();

            decimal dVal = val.DecimalValue.Value;
            Assert.IsTrue(dVal == 456.0m);

            Assert.IsTrue(converter.ErrorCode == 0);

        }

        [TestMethod]
        public void TestAuditFieldDefinition()
        {
            DataDictionary d = new DataDictionary();
            d.PopulateFaked();

            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            var formatService = Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;

            EurekaField source = DataFacade.GetEurekaFieldDefinition(ctx, "TestNative", fmtEureka);

            var logs = DataFacade.GetAuditLogs(ctx);
            int cnt = logs.Count();

            source.Description = "Test field";
            source.CreatedDate = DateTime.Now;

            ctx.SaveChanges();

            int cnt2 = logs.Count();

            Assert.IsTrue(cnt2 > cnt);

        }



    }
}
