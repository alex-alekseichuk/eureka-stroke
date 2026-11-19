using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.ReportService
{
    /// <summary>
    /// Type of specified user object
    /// </summary>
    public enum UserObjectType
    {
        Folder, Report, Facility, ReportingSource
    }

    /// <summary>
    /// User Object is a resource that can be available (assigned) to be used by specified user.
    /// It's like permissions on object.
    /// </summary>
    public class UserObject
    {
        /// <summary>
        /// reportID, facilityID or folderID
        /// it has no sense for ReportingSource because there is just string.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// String name of the object.
        /// It's some kind of ID in case of reporting source type.
        /// </summary>
        public string Name { get; set; }

        public UserObjectType Type { get; set; }
        public string TypeName {
            get
            {
                switch (Type)
                {
                    case UserObjectType.Folder:
                        return "Category";
                    case UserObjectType.Report:
                        return "Report";
                    case UserObjectType.Facility:
                        return "Facility";
                    case UserObjectType.ReportingSource:
                        return "Reporting Source";
                }
                return "";
            }

        }
    }

    public class ReportingSource
    {
        public string Name { get; set; }
    }
}
