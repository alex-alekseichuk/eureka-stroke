using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Eureka.Stroke.Parser
{
    /// <summary>
    /// Temporal overlap verification
    /// </summary>
    public static class TemporalOverlapManager
    {
        public static EnumTemporalOverlapResult Verify(Facility facility, StrokeData data, out string errMsg)
        {
            EnumTemporalOverlapResult res = EnumTemporalOverlapResult.CheckKeyFields;
            errMsg = string.Empty;

            // get key fields
            DateTime admissionDate = data.AdmissionDate;
            string patientID = data.PatientID;

            // implement temporal overlap feature
            DateTime maxAdmissionDate = facility.MaxAdmissionDate;
            int temporalOverlapMonths = Properties.Settings.Default.AdmissionDateOverlapMonths;
            if (maxAdmissionDate > Utils.GetDateMinValue())
            {
                DateTime dateOverlap = maxAdmissionDate.AddMonths(-temporalOverlapMonths);
                // our db max(admission date)-temporal overlap > admission date: we ignore record 
                if (dateOverlap > admissionDate)
                {
                    errMsg = string.Format("Record is ignored due to temporal overlap: PatientID {0}, Admission date {1}", patientID, admissionDate);
                    Logger.Instance.LogDebug(errMsg);
                    Debug.WriteLine(errMsg);
                    res = EnumTemporalOverlapResult.Ignore;
                    
                }

                // admission date > our db max(admission date): treat as a new record without checking for key fields and dups 
                if (admissionDate > maxAdmissionDate)
                {
                    errMsg = string.Format("Record is treated as new: PatientID {0}, Admission date {1}", patientID, admissionDate);
                    Logger.Instance.LogDebug(errMsg);
                    Debug.WriteLine(errMsg);
                    res = EnumTemporalOverlapResult.NewRecord;
                    
                }

            }
            return res;
        }
    }
}
