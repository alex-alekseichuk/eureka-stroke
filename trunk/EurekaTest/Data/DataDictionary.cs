using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke.FormatService;
using Eureka.Stroke.Data;
using System.Diagnostics;
using EurekaTest;

namespace Eureka.Stroke
{
    internal class DataDictionary
    {

        public DataDictionary()
        {
            Initializer.BootStrap();
        }

        public void PopulateFaked()
        {
            EurekaStrokeEntities ctx = Initializer.DIContainer.Resolve<EurekaStrokeEntities>();
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            var formatService = Initializer.DIContainer.Resolve<IFormatService>();
            var fmtEureka = formatService.EurekaFormat; //DataFacade.GetNativeFormat(ctx);

            //Facility facility = DataFacade.GetFacilityByID(ctx, 2);
            AlienField alienField;
            EurekaField eField;
            //FieldDefinitionMap map;

            AlienValue[] aVal = new AlienValue[20];
            EurekaValue[] eVal = new EurekaValue[20];
            //FieldValueDefinitionMap[] valMaps = new FieldValueDefinitionMap[20];

            string fname = "";
            string fnameEureka = "";

            #region test
            fname = "test";
            fnameEureka = "TestNative";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                


                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.SingleSelect, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "JC.Clinical";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                eVal[0] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Yes",
                    Value = "1"
                };

                eVal[1] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "No",
                    Value = "2"
                };

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //                alienField.Description = @"During this hospital stay, was the patient 
                //enrolled in a clinical trial in which patients with the same condition as the 
                //measure set were being studied (i.e. AMI, CAC, HF, PN, PR, SCIP, STK,VTE)? ";
                alienField.Format = fmtAlien;
                //alienField.IsIsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;

                aVal[0] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[0],
                    Format = fmtAlien,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Yes",
                    Value = "1"
                };

                aVal[1] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[1],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Format = fmtAlien,
                    Description = "No",
                    Value = "2"
                };

                

                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion test
            ctx.SaveChanges();

        }

        public void Populate()
        {
            EurekaStrokeEntities ctx = Initializer.DIContainer.Resolve<EurekaStrokeEntities>();
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            // get native format
            var formatService = Initializer.DIContainer.Resolve<IFormatService>();
            var fmtEureka = formatService.EurekaFormat;

            //Facility facility = DataFacade.GetFacilityByID(ctx, 2);
            AlienField alienField;
            EurekaField eField;


            AlienValue[] aVal = new AlienValue[20];
            EurekaValue[] eVal = new EurekaValue[20];
            

            string fname = "";
            string fnameEureka = "";

            #region PatientID
            fname = "patient_os_id";
            fnameEureka = "PatientID";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.String, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "Patient ID";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = "Patient ID";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;

                

                

                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion PatientID

            #region jc_clinical
            fname = "jc_clinical";
            fnameEureka = "JC.Clinical";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.SingleSelect, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "JC.Clinical";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                eVal[0] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Yes",
                    Value = "1"
                };

                eVal[1] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "No",
                    Value = "2"
                };

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = @"During this hospital stay, was the patient 
//enrolled in a clinical trial in which patients with the same condition as the 
//measure set were being studied (i.e. AMI, CAC, HF, PN, PR, SCIP, STK,VTE)? ";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;

                aVal[0] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[0],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Yes",
                    Value = "1",
                    Format = fmtAlien
                };

                aVal[1] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[2],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "No",
                    Value = "2",
                    Format = fmtAlien
                };

                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion jc_clinical

            #region jc_admitdate
            fname = "jc_admitdate";
            fnameEureka = "AdmissionDate";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.DateTime, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "Admission Date";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = "Admission Date";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;





                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion jc_admitdate

            #region jc_arrdatetime
            fname = "jc_arrdatetime";
            fnameEureka = "ArrivalDateTime";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.DateTime, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "Arrival Date";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = "Arrival Date";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;





                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion jc_arrdatetime


            #region gs_symptomlocation
            fname = "gs_symptomlocation";
            fnameEureka = "GS.SymptomLocation";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.SingleSelect, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "Patient location when stroke symptoms discovered";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                eVal[0] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Not in a healthcare setting",
                    Value = "1"
                };

                eVal[1] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Another acute care facility",
                    Value = "2"
                };

                eVal[2] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Chronic health care facility",
                    Value = "3"
                };

                eVal[3] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Outpatient healthcare setting",
                    Value = "4"
                };

                eVal[4] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Stroke occurred while patient was an inpatient in your hospital",
                    Value = "5"
                };


                eVal[5] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "ND or Cannot be determined",
                    Value = "6"
                };

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = @"Patient location when stroke symptoms discovered";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = false;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;

                aVal[0] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[0],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Not in a healthcare setting",
                    Value = "1",
                    Format = fmtAlien
                };

                aVal[1] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[1],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Another acute care facility",
                    Value = "2",
                    Format = fmtAlien
                };

                aVal[2] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[2],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Chronic health care facility",
                    Value = "3",
                    Format = fmtAlien
                };

                aVal[3] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[3],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Outpatient healthcare setting",
                    Value = "5",
                    Format = fmtAlien
                };

                aVal[4] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[4],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Stroke occurred while patient was an inpatient in your hospital",
                    Value = "4",
                    Format = fmtAlien
                };


                aVal[5] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[5],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "ND or Cannot be determined",
                    Value = "9",
                    Format = fmtAlien
                };









                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion gs_symptomlocation

            #region jc_dob
            fname = "jc_dob";
            fnameEureka = "DateOfBirth";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.DateTime, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "Date of Birth";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = "Date of Birth";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;





                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion jc_dob

            #region gs_age
            fname = "gs_age";
            fnameEureka = "Age";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.Integer, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "Age";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = "Age";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;



                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion gs_age

            #region jc_gender
            fname = "jc_gender";
            fnameEureka = "Gender";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.SingleSelect, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "Gender";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                eVal[0] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Male",
                    Value = "1"
                };

                eVal[1] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Female",
                    Value = "2"
                };

                eVal[2] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Unknown",
                    Value = "3"
                };

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = @"Gender";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;

                aVal[0] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[0],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Male",
                    Value = "1",
                    Format = fmtAlien
                };

                aVal[1] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[1],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Female",
                    Value = "2",
                    Format = fmtAlien
                };

                aVal[2] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[2],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Unknown",
                    Value = "3",
                    Format = fmtAlien
                };




                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion jc_gender

            #region gs_race
            fname = "gs_race";
            fnameEureka = "Race";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {

                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.MultipleSelect, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "Gender";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                eVal[0] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "White",
                    Value = "1"
                };

                eVal[1] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Black or African American",
                    Value = "2"
                };

                eVal[2] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Asian",
                    Value = "3"
                };

                eVal[3] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "American Indian/Alaska Native",
                    Value = "4"
                };

                eVal[4] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Native Hawaiian/Pacific Islander",
                    Value = "5"
                };

                eVal[5] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "UTD",
                    Value = "6"
                };

                eVal[6] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Other",
                    Value = "7"
                };


                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = @"Race";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;

                aVal[0] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[0],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "White",
                    Value = "1",
                    Format = fmtAlien
                };

                aVal[1] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[1],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Black or African American",
                    Value = "2",
                    Format = fmtAlien
                };

                aVal[2] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[2],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Asian",
                    Value = "3",
                    Format = fmtAlien
                };

                aVal[3] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[3],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "American Indian/Alaska Native",
                    Value = "4",
                    Format = fmtAlien
                };

                aVal[4] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[4],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Native Hawaiian/Pacific Islander",
                    Value = "5",
                    Format = fmtAlien
                };

                aVal[5] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[5],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "UTD",
                    Value = "6",
                    Format = fmtAlien
                };

                aVal[6] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[6],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Other",
                    Value = "7",
                    Format = fmtAlien
                };





                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion gs_race

            #region gs_stroketype
            fname = "gs_stroketype";
            fnameEureka = "StrokeType";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.SingleSelect, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "Stroke type";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;

                eVal[0] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Ischemic stroke",
                    Value = "2"
                };

                eVal[1] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Transient Ischemic Attack (< 24 hours)",
                    Value = "3"
                };

                eVal[2] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Subarachnoid Hemorrhage",
                    Value = "4"
                };

                eVal[3] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Intracerebral Hemorrhage",
                    Value = "5"
                };

                eVal[4] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Stroke not otherwise specified",
                    Value = "6"
                };

                eVal[5] = new EurekaValue()
                {
                    EurekaField = eField,
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "No stroke related diagnosis",
                    Value = "7"
                };

                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = @"Stroke Type";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;

                aVal[0] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[0],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Ischemic stroke",
                    Value = "2",
                    Format = fmtAlien
                };

                aVal[1] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[1],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Transient Ischemic Attack (< 24 hours)",
                    Value = "3",
                    Format = fmtAlien
                };

                aVal[2] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[2],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Subarachnoid Hemorrhage",
                    Value = "4",
                    Format = fmtAlien
                };

                aVal[3] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[3],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Intracerebral Hemorrhage",
                    Value = "5",
                    Format = fmtAlien
                };

                aVal[4] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[4],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "Stroke not otherwise specified",
                    Value = "6",
                    Format = fmtAlien
                };

                aVal[5] = new AlienValue()
                {
                    EurekaField = eField,
                    EurekaValue = eVal[5],
                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                    CreatedDate = DateTime.Now,
                    Description = "No stroke related diagnosis",
                    Value = "7",
                    Format = fmtAlien
                };








                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion gs_stroketype

            #region gs_hba1c
            fname = "gs_hba1c";
            fnameEureka = "A1C";
            alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
            if (alienField == null)
            {
                eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)EnumFieldType.Decimal, true, true);
                eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                eField.CreatedDate = DateTime.Now;
                eField.Description = "A1C";
                eField.Format = fmtEureka;
                eField.IsMandatory = true;
                eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                eField.LastModifiedDate = DateTime.Now;


                alienField = AlienField.CreateAlienField(0, fname);
                alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                alienField.CreatedDate = DateTime.Now;
                //alienField.Description = "A1C";
                alienField.Format = fmtAlien;
                //alienField.IsMandatory = true;
                alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                alienField.LastModifiedDate = DateTime.Now;
                alienField.EurekaField = eField;



                ctx.AddToAlienField(alienField);
            }
            else Debug.Print("field definition already exists for '" + fname + "'");
            #endregion gs_hba1c




            ctx.SaveChanges();



            
        }
    }
}
