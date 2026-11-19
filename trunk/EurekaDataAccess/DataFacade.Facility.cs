using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke;

namespace Eureka.Stroke.Data
{
    public static partial class DataFacade
    {
        /// <summary>
        /// Get Facility by id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Facility GetFacilityByID(EurekaStrokeEntities ctx, int id)
        {
            var item = (from f in ctx.Facility
                        where f.ID == id
                        select f).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Get Facility by id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Facility GetFacilityByName(EurekaStrokeEntities ctx, string name)
        {
            var item = (from f in ctx.Facility
                        where f.Name == name
                        select f).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Get Facility by format
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        public static Facility GetFacilityByFormat(EurekaStrokeEntities ctx, Format fmt)
        {
            var item = (from f in ctx.Facility
                        where f.Format.ID == fmt.ID
                        select f).FirstOrDefault();
            return item;
        }


        public static Facility AddFacility(EurekaStrokeEntities ctx, string name, Format format)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentOutOfRangeException("name");
            if (null == format)
                throw new ArgumentOutOfRangeException("format");

            var facility = Facility.CreateFacility(0, name);
            ctx.AddToFacility(facility);
            facility.FormatReference.Value = format;
            ctx.SaveChanges();

            return facility;
        }

        public static IList<Facility> GetFacilities(EurekaStrokeEntities ctx)
        {
            return (from facility in ctx.Facility
                    orderby facility.Name
                    select facility).ToList();
        }
        public static IList<Facility> FilterFacilities(EurekaStrokeEntities ctx, string queryString)
        {
            return (from facility in ctx.Facility
                    where facility.Name.Contains(queryString)
                    orderby facility.Name
                    select facility).ToList();
        }


    }
}
