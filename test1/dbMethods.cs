using bd1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace test1
{
    class dbMethods
    {

        public dbMethods()
        {
            using (DBDataContext context = new DBDataContext("Data Source='isostore:/gpsdata.sdf'"))
            {
                if (!context.DatabaseExists())
                {
                    context.CreateDatabase();
                }
            }
        }

        public void addRow(String lat, String lon, String time)
        {
            using (DBDataContext context = new DBDataContext("Data Source='isostore:/gpsdata.sdf'"))
            {
                if (!context.DatabaseExists())
                {
                    context.CreateDatabase();
                }
                gpsData newgpsdata = new gpsData()
                {
                    Latitude = lat,
                    Longitude = lon,
                    Timestamp = time
                };

                context.gpsData.InsertOnSubmit(newgpsdata);
                context.SubmitChanges();
            }
        }

        public List<gpsData> getAllRows()
        {
            using (DBDataContext context = new DBDataContext("Data Source='isostore:/gpsdata.sdf'"))
            {
                List<gpsData> datalist = context.gpsData.ToList();
                return datalist;
            }
        }

        public void delete()
        {
            using (DBDataContext context = new DBDataContext("Data Source='isostore:/gpsdata.sdf'"))
            {
                context.DeleteDatabase();
                context.CreateDatabase();
            }
        }

    }
}
