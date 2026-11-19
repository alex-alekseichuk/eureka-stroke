using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for WebUtils
/// </summary>
public class WebUtils
{
    public static void AvoidHttpCache(HttpResponse response)
    {
        response.ClearHeaders();
        response.AppendHeader("Cache-Control", "no-cache");
        response.AppendHeader("Cache-Control", "private");
        response.AppendHeader("Cache-Control", "no-store");
        response.AppendHeader("Cache-Control", "must-revalidate");
        response.AppendHeader("Cache-Control", "max-stale=0");
        response.AppendHeader("Cache-Control", "post-check=0");
        response.AppendHeader("Cache-Control", "pre-check=0");
        response.AppendHeader("Pragma", "no-cache");
        response.AppendHeader("Keep-Alive", "timeout=3, max=993");
        response.AppendHeader("Expires", "Mon, 26 Jul 1997 05:00:00 GMT");
    }
}
