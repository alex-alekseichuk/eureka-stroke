using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Eureka.Stroke.FormatService;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Eureka.Stroke.Parser;
using Eureka.Stroke;
using Eureka.Stroke.Data;
using System.Diagnostics;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

namespace ExcelDictionaryParser
{
    public partial class Sheet2
    {
        private void Sheet2_Startup(object sender, System.EventArgs e)
        {
            var container = Eureka.Stroke.Container.DIContainer;
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Containers.Default.Configure(container);

            Configurator.BootStrap();
        }

        private void Sheet2_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            this.btnParseStanford.Click += new System.EventHandler(this.btnParseStanford_Click);
            this.Shutdown += new System.EventHandler(this.Sheet2_Shutdown);
            this.Startup += new System.EventHandler(this.Sheet2_Startup);

        }

        #endregion

        private string GetEurekaName(string alien)
        {
            string name = alien;
            switch (alien)
            {
                case "patient_os_id":
                    name = "PatientID"; break;
                case "jc_clinical":
                    name = "JC.Clinical"; break;
                case "jc_admitdate":
                    name = "AdmissionDate"; break;
                case "jc_arrdatetime":
                    name = "ArrivalDateTime"; break;
                case "gs_symptomlocation":
                    name = "GS.SymptomLocation"; break;
                case "jc_dob":
                    name = "DateOfBirth"; break;
                case "gs_age":
                    name = "Age"; break;
                case "jc_gender":
                    name = "Gender"; break;

                case "gs_race":
                    name = "Race"; break;

                case "gs_stroketype":
                    name = "StrokeType"; break;

                case "gs_hba1c":
                    name = "A1C"; break;

                default:
                    StringBuilder sb = new StringBuilder(alien);
                    sb.Replace("_", ".");
                    if (alien.StartsWith("gs"))
                        sb.Replace("gs", "GS");
                    if (alien.StartsWith("jc"))
                        sb.Replace("gs", "JC");

                    // first 2 chars make upper case
                    string start = sb.ToString().Substring(0, 2).ToUpperInvariant();
                    sb[0] = start[0];
                    sb[1] = start[1];

                    // first char after "." make upper case
                    for (int i = 0; i < sb.Length; i++)
                    {
                        char ch = sb[i];
                        if (ch == '.')
                        {
                            if (i + 1 < sb.Length)
                            {
                                ch = sb[i + 1];
                                ch = Char.ToUpperInvariant(ch);
                                sb[i + 1] = ch;
                            }
                        }
                    }

                    name = sb.ToString();

                    break;

            }

            return name;
        }

        private EnumFieldType GetFieldType(string fieldType, int colorInd)
        {
            EnumFieldType type = EnumFieldType.Integer;
            if (fieldType.StartsWith("Alphanumeric"))
            {
                if (colorInd == 6)
                    type = EnumFieldType.SingleSelect;
                else if (colorInd == 39 || colorInd == 36)
                    type = EnumFieldType.MultipleSelect;
                else
                    type = EnumFieldType.String;
            }
            else if (fieldType.StartsWith("ICD-9"))
            {
                type = EnumFieldType.String;
            }
            else if (fieldType.StartsWith("Date"))
            {
                type = EnumFieldType.DateTime;
            }
            else if (fieldType.StartsWith("Int"))
            {
                type = EnumFieldType.Integer;
            }
            else if (fieldType.StartsWith("Decimal"))
            {
                type = EnumFieldType.Decimal;
            }
            else if (fieldType.StartsWith("Numeric"))
            {
                type = EnumFieldType.Decimal;
            }
            else if (fieldType.StartsWith("Single Checkbox"))
            {
                type = EnumFieldType.Boolean;
            }

            else if (colorInd == 6)
                type = EnumFieldType.SingleSelect;
            else if (colorInd == 39 || colorInd == 36)
                type = EnumFieldType.MultipleSelect;
            else
                type = EnumFieldType.String;

            return type;
        }

        // Microsoft.Office.Tools.Excel.Worksheet sheet

        private string GetCellValue(int row, char col, Microsoft.Office.Tools.Excel.Worksheet sheet, bool removeSpace)
        {
            string val = "";
            string cell = col + row.ToString();
            Excel.Range rCell = sheet.get_Range(cell, missing);
            if (rCell.Value2 != null)
                val = rCell.Value2.ToString();
            else
                val = "";
            if (removeSpace)
                val = val.Trim().Replace(" ", "");
            return val;
        }

        private string GetCellValue(int row, char col, bool removeSpace)
        {
            Microsoft.Office.Tools.Excel.Worksheet sheet = Globals.Sheet1;
            string val = GetCellValue(row, col, sheet, removeSpace);
            if (removeSpace)
                val = val.Trim().Replace(" ", "");
            return val;
        }

        private string GetCellValueStanford(int row, char col, bool removeSpace)
        {
            Microsoft.Office.Tools.Excel.Worksheet sheet = Globals.Sheet3;
            string val = GetCellValue(row, col, sheet, removeSpace);
            if (removeSpace)
             val = val.Trim().Replace(" ", "");
            return val;
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            var container = Eureka.Stroke.Container.DIContainer;
            EurekaStrokeEntities ctx = container.Resolve<EurekaStrokeEntities>();
            Format fmtAlien = DataFacade.GetFormatByName(ctx, "Outcome");

            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;

            //Facility facility = DataFacade.GetFacilityByID(ctx, 2);
            AlienField alienField = null;
            EurekaField eField = null;

            AlienValue[] aVal = new AlienValue[200];
            EurekaValue[] eVal = new EurekaValue[200];


            string fname = "";
            string fnameEureka = "";

            int rowStart = 2;
            int rowEnd = 1310;
            Microsoft.Office.Tools.Excel.Worksheet sheet = Globals.Sheet1;

            int indexValue = 0;
            bool isNewField = false;
            EnumFieldType typeEureka = EnumFieldType.Boolean;

            for (int i = rowStart; i <= rowEnd; i++)
            {

                Excel.Range rCell;
                fname = GetCellValue(i, 'E',true);

                string cell = "D" + i.ToString();
                rCell = sheet.get_Range(cell, missing);
                int colorInd = 0;
                try
                {
                    colorInd = (int)rCell.Interior.ColorIndex; // 6 - single line, 39 - multi line
                }
                catch { }

                //if (colorInd != 6 && colorInd!=-4142)
                //Debug.Print(colorInd.ToString());

                string fieldType = GetCellValue(i, 'C',true);
                string valueDesc = GetCellValue(i, 'B',false);
                string valueStatic = GetCellValue(i, 'F',true);
                string fieldDesc = GetCellValue(i, 'A',false);

                bool ignore = valueStatic.Equals("See Appendix 2", StringComparison.InvariantCultureIgnoreCase);


                if (!string.IsNullOrEmpty(fname) && !ignore)
                {
                    isNewField = false;
                    indexValue = 0;

                    fnameEureka = GetEurekaName(fname);
                    EnumFieldType type = GetFieldType(fieldType, colorInd);

                    alienField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);
                    typeEureka = type;

                    //Debug.Print("Found field " + fname + " Color index = " + colorInd + " eureka: " + fnameEureka);

                    if (alienField == null)
                    {
                        isNewField = true;

                        if (fname.Contains("gs_antithrombotic") && fname.Contains("dose"))
                        {
                            typeEureka = EnumFieldType.Decimal;
                        }

                        eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)typeEureka, true, true);
                        eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                        eField.CreatedDate = DateTime.Now;
                        eField.Description = fieldDesc;
                        eField.Format = fmtEureka;
                        eField.IsMandatory = true;
                        eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                        eField.LastModifiedDate = DateTime.Now;




                        alienField = AlienField.CreateAlienField(0, fname);
                        alienField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                        alienField.CreatedDate = DateTime.Now;
                        //alienField.Description = fieldDesc;
                        alienField.Format = fmtAlien;
                        //alienField.IsMandatory = true;
                        alienField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                        alienField.LastModifiedDate = DateTime.Now;
                        alienField.EurekaField = eField;


                        // \d+\.\d+
                        if (fname.Contains("gs_antithrombotic") && fname.Contains("dose"))
                        {
                            alienField.RegularExpression = @"\d+\.?\d*";
                        }




                        ctx.AddToAlienField(alienField);

                        Debug.Print("NEW field " + fname + " eureka: " + fnameEureka + " type: " + type.ToString());



                    }
                    else
                    {
                        alienField.EurekaFieldReference.EnsureLoad();
                        eField = alienField.EurekaField;

                        if (eField.FieldTypeEnum == EnumFieldType.String && typeEureka == EnumFieldType.MultipleSelect)
                        {
                            eField.FieldTypeEnum = typeEureka;
                            isNewField = true;
                            Debug.Print("FIXED Eureka field type to MultiValue '" + fname + "', " + " '" + fnameEureka + "'");
                        }
                        else
                        {

                            isNewField = false;
                            //Debug.Print("field definition already exists for '" + fname + "'");
                        }
                    }


                }
                else if (alienField != null && isNewField)
                {
                    if (typeEureka == EnumFieldType.SingleSelect
                        ||
                        typeEureka == EnumFieldType.MultipleSelect || typeEureka == EnumFieldType.Decimal)
                    {
                        // fill values
                        if (!string.IsNullOrEmpty(valueStatic))
                        {
                            //if (eField.FieldTypeEnum == EnumFieldType.SingleSelect
                            //||
                            //eField.FieldTypeEnum == EnumFieldType.MultipleSelect)
                            //{
                            eVal[indexValue] = new EurekaValue()
                            {
                                EurekaField = eField,
                                CreatedBy = (int?)DataFacade.GetCurrentUser(),
                                CreatedDate = DateTime.Now,
                                Description = valueDesc,
                                Value = valueStatic
                            };
                            ctx.AddToEurekaValue(eVal[indexValue]);

                            //}

                            aVal[indexValue] = new AlienValue()
                            {
                                EurekaField = eField,
                                EurekaValue = eVal[indexValue],
                                CreatedBy = (int?)DataFacade.GetCurrentUser(),
                                CreatedDate = DateTime.Now,
                                Description = valueDesc,
                                Value = valueStatic,
                                Format = fmtAlien
                            };

                            ctx.AddToAlienValue(aVal[indexValue]);


                            indexValue++;
                        }
                    }
                }



            } // end loops
            ctx.SaveChanges();

        }

        private void btnParseStanford_Click(object sender, EventArgs e)
        {
            var container = Eureka.Stroke.Container.DIContainer;
            EurekaStrokeEntities ctx = container.Resolve<EurekaStrokeEntities>();
            Format fmtStanford = DataFacade.GetFormatByName(ctx, "Stanford");
            Format fmtOutcome = DataFacade.GetFormatByName(ctx, "Outcome");

            if (fmtStanford == null)
            {
                fmtStanford = new Format()
                {
                    IsNative = false,
                    KeyFields = "patient_os_id,jc_admitdate",
                    Name = "Stanford",
                    Status = 0
                };
                ctx.AddToFormat(fmtStanford);
                ctx.SaveChanges();
            }

            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
            Format fmtEureka = formatService.EurekaFormat;


            AlienField outcomeField = null;
            AlienField stanfordField = null;
            EurekaField eField = null;

            AlienValue[] outcomeVal = new AlienValue[200];
            AlienValue[] stanfordVal = new AlienValue[200];
            EurekaValue[] eVal = new EurekaValue[200];


            string fname = "";
            string fnameEureka = "";

            int rowStart = 2;
            int rowEnd = 1310;
            Microsoft.Office.Tools.Excel.Worksheet sheet = Globals.Sheet3;


            int indexValue = 0;
            bool isNewField = false;
            bool notMapped = false;
            EnumFieldType typeEureka = EnumFieldType.Boolean;
            //Dictionary<int, string> map = new Dictionary<int, string>();

            for (int i = rowStart; i <= rowEnd; i++)
            {

                Excel.Range rCell;
                string cell = "A" + i.ToString();
                rCell = sheet.get_Range(cell, missing);
                int colorInd = 0;
                try
                {
                    colorInd = (int)rCell.Interior.ColorIndex;
                    //map[colorInd] = cell;
                    notMapped = colorInd == 7;
                }
                catch { }


                string fieldIndex = GetCellValueStanford(i, 'A',false);
                string stanfordFieldDesc = GetCellValueStanford(i, 'B',false);
                string stanfordFieldName = GetCellValueStanford(i, 'C',true);

                string stanfordValue = GetCellValueStanford(i, 'C',true);
                string stanfordValueDesc = GetCellValueStanford(i, 'D',false);

                string stanfordType = GetCellValueStanford(i, 'D',false);

                string outcomeFieldDesc = GetCellValueStanford(i, 'E',false);
                string outcomeFieldName = GetCellValueStanford(i, 'F',true);
                string outcomeValue = GetCellValueStanford(i, 'G',true);
                string outcomeValueDesc = GetCellValueStanford(i, 'H',false);

                bool isFieldAvailable = !string.IsNullOrEmpty(fieldIndex);

                if (notMapped)
                {
                    Debug.Print("Field " + outcomeFieldName + " NOT MAPPED (PINK)");
                    break;
                }

                if (isFieldAvailable && !string.IsNullOrEmpty(outcomeFieldName))
                {
                    eField = null;
                    indexValue = 0;
                    isNewField = false;
                    if (string.IsNullOrEmpty(stanfordFieldName) || stanfordFieldName == "???")
                        stanfordFieldName = outcomeFieldName;

                    stanfordField = DataFacade.GetAlienFieldDefinition(ctx, stanfordFieldName, fmtStanford);
                    if (stanfordField == null)
                    {
                        outcomeField = DataFacade.GetAlienFieldDefinition(ctx, outcomeFieldName, fmtOutcome);
                        if (outcomeField != null)
                        {
                            eField = outcomeField.EurekaField;
                            if (eField != null)
                            {
                                typeEureka = eField.FieldTypeEnum;
                                isNewField = true;

                                stanfordField = AlienField.CreateAlienField(0, stanfordFieldName);
                                stanfordField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                                stanfordField.CreatedDate = DateTime.Now;
                                stanfordField.Format = fmtStanford;
                                stanfordField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                                stanfordField.LastModifiedDate = DateTime.Now;
                                stanfordField.EurekaField = eField;

                                ctx.AddToAlienField(stanfordField);

                                Debug.Print("NEW stanford field " + stanfordFieldName + " eureka: " + eField.Name + " type: " + typeEureka.ToString());

                            }
                            else
                            {
                                Debug.Print("Eureka field for Outcome " + outcomeFieldName + " does not exist ");

                            }

                        }
                        else
                        {
                            Debug.Print("Outcome field " + outcomeFieldName + " does not exist ");
                        }
                    }
                    else
                    {
                        Debug.Print("Stanford field " + stanfordFieldName + " already exists ");
                    }
                    //outcomeField = DataFacade.GetAlienFieldDefinition(ctx, fname, fmtAlien);

                }

                if (!string.IsNullOrEmpty(stanfordFieldName) && isFieldAvailable && string.IsNullOrEmpty(outcomeFieldName))
                {
                    typeEureka = EnumFieldType.SingleSelect;
                    if (stanfordType == "Y/N") typeEureka = EnumFieldType.Boolean;
                    else if (stanfordType == "Date") typeEureka = EnumFieldType.DateTime;

                    //Debug.Print("!!!!!!!!!!!!!!!!!!!!!!Stanford field " + stanfordFieldName + " exists, but Outcome is missing, type = " + typeEureka.ToString() + " Creating...");
                    stanfordField = DataFacade.GetAlienFieldDefinition(ctx, stanfordFieldName, fmtStanford);

                    if (stanfordField == null)
                    {
                        isNewField = true;
                        fnameEureka = GetEurekaName(stanfordFieldName);
                        eField = EurekaField.CreateEurekaField(0, fnameEureka, (short)typeEureka, true, true);
                        eField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                        eField.CreatedDate = DateTime.Now;
                        eField.Description = stanfordFieldDesc;
                        eField.Format = fmtEureka;
                        eField.IsMandatory = true;
                        eField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                        eField.LastModifiedDate = DateTime.Now;
                        
                        stanfordField = AlienField.CreateAlienField(0, stanfordFieldName);
                        stanfordField.CreatedBy = (int?)DataFacade.GetCurrentUser();
                        stanfordField.CreatedDate = DateTime.Now;
                        //alienField.Description = fieldDesc;
                        stanfordField.Format = fmtStanford;
                        //alienField.IsMandatory = true;
                        stanfordField.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                        stanfordField.LastModifiedDate = DateTime.Now;
                        stanfordField.EurekaField = eField;

                        ctx.AddToAlienField(stanfordField);
                        Debug.Print("NEW stanford field " + stanfordFieldName + " eureka: " + eField.Name + " type: " + typeEureka.ToString());
                    }
                    else
                    {
                        Debug.Print("Stanford field " + stanfordFieldName + " already exists ");
                    }

                }

                // map values
                if (stanfordField != null && outcomeField != null && isNewField && !isFieldAvailable)
                {
                    if (typeEureka == EnumFieldType.SingleSelect
                        ||
                        typeEureka == EnumFieldType.MultipleSelect || typeEureka == EnumFieldType.Decimal)
                    {

                        // fill values
                        if (!string.IsNullOrEmpty(outcomeValue) && eField != null && !string.IsNullOrEmpty(stanfordValue))
                        {

                            outcomeVal[indexValue] = DataFacade.GetAlienFieldValueByValue(ctx, outcomeValue, eField.ID, fmtOutcome.ID);
                            if (outcomeVal[indexValue] != null)
                            {
                                outcomeVal[indexValue].EurekaValueReference.EnsureLoad();
                                eVal[indexValue] = outcomeVal[indexValue].EurekaValue;
                                stanfordVal[indexValue] = new AlienValue()
                                {
                                    EurekaField = eField,
                                    EurekaValue = eVal[indexValue],
                                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                                    CreatedDate = DateTime.Now,
                                    Description = stanfordValueDesc,
                                    Value = stanfordValue,
                                    Format = fmtStanford
                                };

                                ctx.AddToAlienValue(stanfordVal[indexValue]);
                            }
                            else
                            {
                                Debug.Print("Outcome value " + outcomeValue + " not found. Field " + eField.Name);
                            }

                            //eVal[indexValue] = new EurekaValue()
                            //{
                            //    EurekaField = eField,
                            //    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                            //    CreatedDate = DateTime.Now,
                            //    Description = valueDesc,
                            //    Value = valueStatic
                            //};
                            //ctx.AddToEurekaValue(eVal[indexValue]);

                            //}




                            indexValue++;
                        }
                        else if (string.IsNullOrEmpty(outcomeValue) && eField != null && !string.IsNullOrEmpty(stanfordValue))
                        {
                            Debug.Print("Outcome value missing. Stanford value = " + stanfordValue + " Field " + eField.Name);
                        }
                        else if (!string.IsNullOrEmpty(outcomeValue) && eField != null && string.IsNullOrEmpty(stanfordValue))
                        {
                            // TO DO:
                            //Debug.Print("!!!!!!!!!!!!!!!Stanford value missing. Outcome value = " + outcomeValue + " Field " + eField.Name);
                            outcomeVal[indexValue] = DataFacade.GetAlienFieldValueByValue(ctx, outcomeValue, eField.ID, fmtOutcome.ID);
                            if (outcomeVal[indexValue] != null)
                            {
                                outcomeVal[indexValue].EurekaValueReference.EnsureLoad();
                                eVal[indexValue] = outcomeVal[indexValue].EurekaValue;
                                stanfordVal[indexValue] = new AlienValue()
                                {
                                    EurekaField = eField,
                                    EurekaValue = eVal[indexValue],
                                    CreatedBy = (int?)DataFacade.GetCurrentUser(),
                                    CreatedDate = DateTime.Now,
                                    Description = eVal[indexValue].Description,
                                    Value = outcomeValue,
                                    Format = fmtStanford
                                };

                                ctx.AddToAlienValue(stanfordVal[indexValue]);
                            }
                            else
                            {
                                Debug.Print("Outcome value " + outcomeValue + " not found. Field " + eField.Name);
                            }
                            indexValue++;

                        }
                        else if (eField == null)
                        {
                            Debug.Print("Eureka Field not mapped  for Stanford " + stanfordField.Name + " Outcome: " + outcomeField.Name);
                        }
                        else
                        {
                            Debug.Print("-----------------Hmmmm! = " + " eureka Field  = " + eField.Name + " Stanford = " + stanfordField.Name + " Outcome: " + outcomeField.Name + ", Stanford Value = " + stanfordValue + " Outcome value = " + outcomeValue);
                            //outcomeValue = GetCellValueStanford(i, 'G');
                        }
                    }
                }




            } // end loop

            //Debug.Print(map.ToString());
            Debug.Print("1");
            ctx.SaveChanges();
        }

    }
}
