using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Data
{
    /// <summary>
    /// Data access routines
    /// </summary>
    public static partial class DataFacade
    {
        /// <summary>
        /// Get first alien format
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static Format GetAlienFormat(EurekaStrokeEntities ctx)
        {
            var item = (from f in ctx.Format
                        where !f.IsNative.Value
                        select f).First();
            return item;
        }

        /// <summary>
        /// Get Format by ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Format GetFormatByID(EurekaStrokeEntities ctx, int id)
        {
            var item = (from f in ctx.Format.Include("KeyEurekaField")
                        select f).Where(f => f.ID == id).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Get Format by ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Format GetFormatByName(EurekaStrokeEntities ctx, string name)
        {
            var item = (from f in ctx.Format
                        where f.Name == name && (f.Status == 0 || f.Status == 1)
                        select f).FirstOrDefault();
            return item;
        }


        /// <summary>
        /// Get All non-native Formats
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static IList<Format> GetFormats(EurekaStrokeEntities ctx)
        {
            var list = (from format in ctx.Format
                        where !format.IsNative.Value && (format.Status == 0 || format.Status == 1)
                        orderby format.Name
                        select format).ToList();
            return list;
        }


        public static Format AddFormat(EurekaStrokeEntities ctx, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentOutOfRangeException("name");

            var format = Format.CreateFormat(0, name);
            format.IsNative = false;
            format.Status = 0;
            ctx.AddToFormat(format);
            ctx.SaveChanges();

            return format;
        }

        public static void SetFormatStatus(EurekaStrokeEntities ctx, Format format, short status)
        {
            if (null == format)
                throw new ArgumentOutOfRangeException("format");
            if (status < 0 || status > 2)
                throw new ArgumentOutOfRangeException("status");

            format.Status = status;
            ctx.SaveChanges();
        }

    }
}
