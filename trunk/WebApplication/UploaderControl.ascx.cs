using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Eureka.Stroke;
using Eureka.Stroke.Data;
using Eureka.Stroke.Parser;
using Eureka.Stroke.UploaderService;
using System.Text;

namespace WebApplication
{
    public partial class UploaderControl : System.Web.UI.UserControl
    {
        protected string jsonUploadLog = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            // init the list of UploadLog's
            var list = UploaderWebService.GetUploadLogList();
            jsonUploadLog = list.ToJSON();
        }

        /// <summary>
        /// Download all stroke data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownloadStrokeData_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}.csv\";",
                "StrokeData"));
            Response.ContentType = "text/csv";
            var data = DataFacade.GetPivotStrokeData();
            if (null != data)
                WriteCsvToOutput(Response.OutputStream, data);
            Response.End();
        }

        /// <summary>
        /// Download specified Facility data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownloadFacilityData_Click(object sender, EventArgs e)
        {
            // get facility ID as HTTP parameter
            var facilityID = hidFacilityID.Value.ToInt();
            if (0 == facilityID)
                return;

            // use uploader service to get facility
            var uploaderService = Eureka.Stroke.Container.DIContainer.Resolve<IUploaderService>();

            // get facility by ID
            var facility = uploaderService.GetFacility(facilityID);
            if (null == facility)
                return;

            Response.Clear();
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"FacilityData_{0}.csv\";",
                facility.Name));
            Response.ContentType = "text/csv";
            var data = DataFacade.GetPivotFacilityData(facilityID);
            if (null != data)
                WriteCsvToOutput(Response.OutputStream, data);
            Response.End();
        }

        /// <summary>
        /// Write CSV to output stream
        /// </summary>
        /// <param name="stream"></param>
        private void WriteCsvToOutput(Stream stream, StrokePivotData data)
        {
            var sbContent = new StringBuilder();
            sbContent.Append(CSVHelper.GetLine(data.Header.ToArray()));
            sbContent.Append(CSVHelper.CRLF);

            foreach (var row in data.Rows)
            {
                sbContent.Append(CSVHelper.GetLine(row.ToArray()));
                sbContent.Append(CSVHelper.CRLF);
            }

            var content = sbContent.ToString();

            var bytes = Encoding.UTF8.GetBytes(content);
            stream.Write(bytes, 0, bytes.Length);

        }
    }
}
