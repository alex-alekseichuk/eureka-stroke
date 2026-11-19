using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Eureka.Stroke.Data;
using System.Text;
using Eureka.Stroke;

/// <summary>
/// File Uploader Web UI.
/// It's used to see uploads and upload new CSV for parsing.
/// </summary>
public partial class Uploader : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WebUtils.AvoidHttpCache(Response);
    }

    
}
