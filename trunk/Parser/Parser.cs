using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Eureka.Stroke.Data;
using Eureka.Stroke.Converter;
using System.Data;
using System.Diagnostics;
using Eureka.Stroke.FormatService;

namespace Eureka.Stroke.Parser
{
    /// <summary>
    /// Eureka CSV parser implementation
    /// </summary>
    public class Parser : BaseService, IParser
    {
        const bool saveToDb = true;
        const char separator = ',';
        //readonly string dbConnString;
        protected string[] fieldNames = null;
        protected UploadLog uploadLog = null;
        protected int numberColumns = 0; // number of columns in the file
        EurekaStrokeEntities ctx = null;
        //protected IConverter converter = null;

        /// <summary>
        /// Create data context
        /// </summary>
        /// <returns></returns>
        protected EurekaStrokeEntities GetDataContext()
        {
            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return ctx;
        }

        
        public Parser(EurekaStrokeEntities ctx)
        {
            this.ctx = ctx;
            DuplicateResolver resolver = new DuplicateResolver();
            Container.DIContainer.RegisterInstance<IDuplicateResolver>(resolver);
        }



        #region IParser Members

        /// <summary>
        /// Parse a file
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="uploadLog"></param>
        /// <returns></returns>
        public Result Parse(Mode mode, UploadLog uploadLog)
        {
            Result res = Result.Error;
            #region file validation
            if (uploadLog == null)
            {
                ErrorMessage = "Upload log is missing";
                return res;
            }



            this.uploadLog = uploadLog;
            this.uploadLog.Status = (short)Result.InProgress;
            if (uploadLog.ErrorCount == null) uploadLog.ErrorCount = 0;
            if (uploadLog.TotalCount == null) uploadLog.TotalCount = 0;
            if (uploadLog.ChangesCount == null) uploadLog.ChangesCount = 0;
            if (uploadLog.CorrectionsCount == null) uploadLog.CorrectionsCount = 0;
                        

            // TODO: save uploadLog
            if (uploadLog.EntityState == System.Data.EntityState.Added)
                ctx.AddToUploadLog(uploadLog);
            SaveBatch();

            // get selected facility and format
            uploadLog.FacilityReference.EnsureLoad();
            Facility facility = uploadLog.Facility;
            if (facility == null)
            {
                ErrorMessage = "Facility is missing";
                this.uploadLog.Status = (short)res;
                SaveBatch();
                return res;
            }

            facility.FormatReference.EnsureLoad();
            Format format = facility.Format;
            if (format == null)
            {
                ErrorMessage = "Format is missing";
                this.uploadLog.Status = (short)res;
                SaveBatch();
                return res;
            }

            if (uploadLog.Format == null)
                uploadLog.Format = format;


            // get filename to be parsed
            string filename = uploadLog.Filename;
            if (string.IsNullOrEmpty(filename))
            {
                ErrorMessage = "Upload file name is missing";
                this.uploadLog.Status = (short)res;
                SaveBatch();
                return res;
            }

            // append path
            string path = Config.Instance.UploaderFolder;
            filename = Path.Combine(path, filename);
            bool exists = File.Exists(filename);
            if (!exists)
            {
                ErrorMessage = string.Format("Upload file {0} is missing", filename);
                this.uploadLog.Status = (short)res;
                SaveBatch();
                return res;
            }
            #endregion validation

            // number of lines to be saved in batch
            int linesInBatch = Properties.Settings.Default.LinesInBatch;

            // read file line by line
            int counter = 0;
            string line;

            // Read the file and display it line by line.
            using (StreamReader file = new StreamReader(filename))
            {
                if (mode == Mode.Online)
                {
                    int maxLinesInOnlineMode = Properties.Settings.Default.MaxLinesInOnlineMode;

                    long linesTotal = Utils.GetNumberOfLines(file, Properties.Settings.Default.MaxLinesInOnlineMode);
                    if (linesTotal > maxLinesInOnlineMode)
                    {
                        ErrorMessage = string.Format("Uploaded file is too big to be processed in online mode, scheduled for later execution by offline parser");
                        res = Result.NeedOffline;
                        this.uploadLog.Status = (short)res;
                        SaveBatch();
                        return res;
                    }
                }

                file.BaseStream.Position = 0;
                int saveCounter = 0;
                while ((line = file.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (counter == 0)
                            ParseHeader(line);
                        else
                        {
                            ParseLine(line,counter);
                            saveCounter++; // count number of rows in batch
                        }
                    }
                    counter++;
                    
                    if (saveCounter >= linesInBatch)
                    {
                        SaveBatch(); // save batch
                        saveCounter = 0;
                    }
                }

                //file.Close();
            }

            if (uploadLog.TotalCount > 0) res = Result.Done;
            uploadLog.Status = (short)res;
            uploadLog.CompletionDate = DateTime.Now;
            SaveBatch();
                        
            ErrorCode = (int)res;
            ErrorMessage = res.ToString();

            // reset Max admission date for the next upload session
            facility.ResetMaxAdmissionDate();

            return res;

        }

        private void SaveBatch()
        {
            // TODO: save uploadLog
            try
            {
                if (saveToDb)
                    ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogActionException("error saving data", ex);
            }
        }





        #endregion

        #region protected
        /// <summary>
        /// Parse header
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        protected Result ParseHeader(string line)
        {
            Result res = Result.Done;
            fieldNames = line.Split(separator);
            if (fieldNames != null)
                numberColumns = fieldNames.Length;
            return res;
        }

        /// <summary>
        /// Returns true if a field is a key field
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        //protected bool IsKeyField(string filename)
        //{
        //    return filename == "patient_os_id";
        //}


        /// <summary>
        /// Parse one line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        protected Result ParseLine(string line, int rowNumber)
        {
            var res = Result.Done; // by default result is Done

            string[] values = null;
            string msg;

            // split the line into several values
            values = line.Split(separator);
            if (values == null)
            {
                res = Result.Error;
                msg = string.Format("(row #{0}) ParseLine - error: no data {1}", rowNumber, line);
                Debug.Print(msg);
                Logger.Instance.LogDebug(msg);
                return res;
            }

            // check that number of fields match number of columns
            if (values.Length != numberColumns)
            {
                res = Result.Error;
                msg = string.Format("(row #{0}) ParseLine - error: number of fields mismatch, expected: {1}, actual: {2}", rowNumber, numberColumns, values.Length);
                Debug.Print(msg);
                Logger.Instance.LogDebug(msg);
                return res;
            }

            // instantiate new StrokeData object to hold parsed values
            var dataNew = StrokeData.CreateStrokeData(0,"",Utils.GetDateMinValue());
            dataNew.CreatedBy = (int?)DataFacade.GetCurrentUser();
            dataNew.CreatedDate = DateTime.Now;

            //ctx.AddToStrokeData(dataNew);

            // get current (alien) format
            var format = GetFormat(this.uploadLog);
            
            // get native format
            var formatService = Container.DIContainer.Resolve<IFormatService>();
            var formatEureka = formatService.EurekaFormat;

            // facility which we uploaded the data under
            var facility = uploadLog.Facility;

            var errCnt = 0;
            var successCnt = 0;

            // enumerate all fields in a row
            for (var i = 0; i < values.Length; i++)
            {
                // trim the value
                var valueToConvert = values[i].Trim(@"\ """.ToCharArray());
                if (string.IsNullOrEmpty(valueToConvert))
                    continue;

                var fieldName = fieldNames[i];

                //EnumKeyField key = GetKeyField(fieldName, format);
                //Action<StrokeData, StrokeFieldValue> actionKey = null;
                //if (key != EnumKeyField.None)
                //{
                //    actionKey = GetKeyFieldAction(key);
                //}


                IEnumerable<StrokeFieldValue> strokeValues;

                // parse one value
                EnumErrorProcessingType errorProcessingType;

                // parse one alien value into one or several stroke values
                var errorType = ParseValue(fieldName, valueToConvert, rowNumber, out strokeValues, out errorProcessingType);

                uploadLog.TotalCount++;
                if (errorType != EnumErrorType.None)
                {
                    if (errorProcessingType == EnumErrorProcessingType.Capture)
                    {
                        //uploadLog.ErrorCount++;
                        errCnt++;
                    }
                    if (strokeValues != null)
                    {
                        strokeValues.ForEach(v => v.Status = (int)Result.Error);
                    }
                }
                else
                    successCnt++;

                if (strokeValues != null)
                {
                    int valueCnt = 0;
                    EnumTemporalOverlapResult resTempOverlap = EnumTemporalOverlapResult.NewRecord;
                    
                    foreach(StrokeFieldValue strokeValue in strokeValues)
                    {
                        var fieldNameEureka = strokeValue.EurekaField.Name;

                        if (valueCnt == 0)
                        {
                            if (fieldNameEureka == "PatientID")
                                dataNew.PatientID = strokeValue.StringValue;
                            if (fieldNameEureka == "AdmissionDate")
                            {
                                dataNew.AdmissionDate = strokeValue.DateTimeValue ?? Utils.GetDateMinValue();
                                resTempOverlap = TemporalOverlapManager.Verify(facility, dataNew, out msg);
                            }
                            if (fieldNameEureka == "ReportingSource")
                                dataNew.ReportingSource = strokeValue.StringValue;
                        }

                        dataNew.StrokeFieldValue.Add(strokeValue);
                        valueCnt++;
                    }
                    if (resTempOverlap == EnumTemporalOverlapResult.Ignore)
                    {
                        break;
                    }

                }

            } // end loop by column

            // check if all key fields are available
            bool dateRecordValid = true;
            string errMsg = "";

            var hasAllKeyFields = true;
            foreach (var ef in format.KeyEurekaField)
            {
                if (0 == dataNew.StrokeFieldValue.Where(v => v.EurekaField == ef).Count())
                {
                    hasAllKeyFields = false;

                    var keyField = ef.Name;
                    msg = string.Format("(row #{0}) ParseLine - error: value for key field(s) {1} are missing or invalid", rowNumber, keyField);
                    Debug.Print(msg);
                    Logger.Instance.LogDebug(msg);

                    DataFacade.CreateAuditLog(ctx, (int)res,
                            false,
                            uploadLog,
                            keyField,
                            msg, "", rowNumber);
                    
                }
            }

            if (!hasAllKeyFields)
            {
                res = Result.Error;
                dateRecordValid = false;
                //string keyField = string.Empty;
                //if (string.IsNullOrEmpty(dataNew.PatientID)) keyField = "PatientID";
                //if (dataNew.AdmissionDate == Utils.GetDateMinValue()) keyField = string.IsNullOrEmpty(keyField) ? "AdmissionDate" : keyField + ", AdmissionDate";


                // remove source values from object context to avoid them being added to db
                var fields = dataNew.StrokeFieldValue.ToList();
                foreach (var v in fields)
                {
                    try
                    {
                        ctx.Detach(v);
                    }
                    catch (Exception ex2)
                    {
                        Logger.Instance.LogException("error detach StrokeValue", ex2);
                    }
                }
                try
                {
                    ctx.Detach(dataNew);
                }
                catch (Exception ex3)
                {
                    Logger.Instance.LogException("error detach StrokeData", ex3);
                }
                errMsg = " NOT SAVED BECAUSE KEY FIELDS ARE MISSING";
                errCnt++;

            }

            if (uploadLog.ErrorCount == null) uploadLog.ErrorCount = 0;
            if (errCnt > 0)
                uploadLog.ErrorCount += errCnt;


            // check for duplicate record
            if (dateRecordValid)
            {
                IDuplicateResolver duplicateResolver = Container.DIContainer.Resolve<IDuplicateResolver>();
                StrokeData data = duplicateResolver.Resolve(dataNew, this.uploadLog);
                if (duplicateResolver.ErrorCode == 0)
                {
                    data.Status = (short)(errCnt > 0 ? 1 : 0);
                    if (data.EntityState == EntityState.Added || data.EntityKey == null || data.EntityKey.IsTemporary)
                    {
                        ctx.AddToStrokeData(data);
                    }
                    else
                    {
                        data.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                        data.LastModifiedDate = DateTime.Now;
                    }
                    uploadLog.StrokeData.Add(data);

                    // execute business rules validator
                    Result resRules = BusinessRuleValidator.Validate(data, uploadLog, rowNumber);
                    if (resRules != Result.Done) errCnt++;
                } // otherwise ignore a record
                else if (duplicateResolver.TemporalOverlapResult== EnumTemporalOverlapResult.Ignore)
                {
                    Debug.WriteLine("Record ignored");
                    dateRecordValid = false;
                    errMsg = duplicateResolver.ErrorMessage;
                }
            }

            // reset Fields definition storage if fields definitions had been updated
            bool fieldDefinitionsAdded = ctx.ExistAffectedEntity<AlienField>(EntityState.Added);
            if (fieldDefinitionsAdded)
            {
                AlienFieldDefinitionCache.Reset();
            }

            msg = string.Format("(ROW #{0}) ParseLine - errors: {1} succesfully imported: {2} {3}", rowNumber, errCnt, successCnt, dateRecordValid?"":errMsg);
            Debug.Print(msg);
            Logger.Instance.LogDebug(msg);

            return res;
        }

        /// <summary>
        /// Get current format for upload session
        /// </summary>
        /// <param name="uploadLog"></param>
        /// <returns></returns>
        private Format GetFormat(UploadLog uploadLog)
        {
            Facility facility = this.uploadLog.Facility;
            Format format = facility.Format;
            return format;
        }



        /// <summary>
        /// Parse one value
        /// </summary>
        /// <param name="fieldName">Alien format field name</param>
        /// <param name="value">raw value to parse</param>
        /// <param name="strokeField">out stroke field value parsed</param>
        /// <returns></returns>
        protected EnumErrorType ParseValue(string fieldName, string value, int rowNumber, out IEnumerable<StrokeFieldValue> strokeFields, out EnumErrorProcessingType errorProcessingType)
        {
            EnumErrorType res = EnumErrorType.None;
            string msg = string.Empty;
            //Facility facility = this.uploadLog.Facility;
            Format format = GetFormat(this.uploadLog);

            ConverterRequest req = new ConverterRequest(fieldName, format, value);

            // get converter instance
            IConverter converter = Container.DIContainer.Resolve<IConverter>();

            ConverterResponse resp = converter.ConvertValue(req);

            res = resp.ErrorType;
            errorProcessingType = resp.ErrorProcessingType;
            //res = resp.IsValid ? Result.Done : Result.Error;

            if (resp.IsValid)
            {
                msg = string.Format("------------(row #{0}) OK ParseValue - field: {1} value: {2} Warning: {3} ", rowNumber, fieldName, value, resp.ErrorMessage);
                Debug.Print(msg);
                Logger.Instance.LogDebug(msg);
            }
            else
            {
                msg = string.Format("------------(row #{0}) NOK ParseValue - field: {1} value: {2} Error: {3} ", rowNumber, fieldName, value, resp.ErrorMessage);
                Debug.Print(msg);
                Logger.Instance.LogDebug(msg);
        
            }
            strokeFields = resp.StrokeValues;

            // create Audit Log records
            CreateAudit(resp, fieldName, value, rowNumber);

            return res;
        }

        /// <summary>
        /// Create Audit trail per value
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="fieldName"></param>
        /// <param name="rowNumber"></param>
        private void CreateAudit(ConverterResponse resp, string fieldName, string value, int rowNumber)
        {
            var strokeFields = resp.StrokeValues;
            var res = resp.IsValid ? Result.Done : Result.Error;

            EnumErrorType errorType = resp.ErrorType;
            EnumErrorProcessingType errorProcessingType = resp.ErrorProcessingType;

            // create Audit Log records
            if (errorProcessingType == EnumErrorProcessingType.Capture)
            {
                if (strokeFields != null && strokeFields.Count() > 0)
                {
                    // only capture "Create" action for error
                    if (res == Result.Error)
                    {
                        foreach (StrokeFieldValue strokeValue in strokeFields)
                        {
                            AuditLog log = DataFacade.CreateAuditLog(ctx, (int)res,
                                resp.IsValid,
                                strokeValue,
                                this.uploadLog,
                                 fieldName,
                                 resp.ErrorMessage, rowNumber);
                            //Debug.Print("Audit");

                        }
                    }
                }
                else
                {
                    // only capture "Create" action for error
                    if (res == Result.Error)
                    {
                        AuditLog log = DataFacade.CreateAuditLog(ctx, (int)res,
                                resp.IsValid,
                                this.uploadLog,
                                 fieldName,
                                 resp.ErrorMessage, value, rowNumber);
                        //Debug.Print("Audit");
                    }
                }
            }
        }
        #endregion

    }
}
