using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WebApplication
{

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

    public static class Util
    {
        private static Control _FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;
            foreach (Control control in root.Controls)
            {
                var c = _FindControlRecursive(control, id);
                if (null != c)
                    return c;
            }
            return null;
        }
        public static Control FindControlRecursive(this Control root, string id)
        {
            return _FindControlRecursive(root, id);
        }

        // TODO correct these methods
        public static int UserID()
        {
            return 1;
        }
        public static bool IsAdmin()
        {
            return true;
        }

    }
}


