using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test1
{
    class dbJsonMethods
    {
         public dbJsonMethods()
        {
            using (DBJsonDataContext context = new DBJsonDataContext("Data Source='isostore:/gpsdataJson.sdf'"))
            {
                if (!context.DatabaseExists())
                {
                    context.CreateDatabase();
                }
            }
        }

        public void addRow(String positionintime)
        {
            using (DBJsonDataContext context = new DBJsonDataContext("Data Source='isostore:/gpsdataJson.sdf'"))
            { 
                if (!context.DatabaseExists())
                {
                    context.CreateDatabase();
                }
                gpsDataJasonTable newgpsjsondata = new gpsDataJasonTable()
                {
                    position = positionintime
                };

               
               
                context.gpsData.InsertOnSubmit(newgpsjsondata);
                context.SubmitChanges();
            }
        }

        public List<gpsDataJasonTable> getAllRows()
        {
            using (DBJsonDataContext context = new DBJsonDataContext("Data Source='isostore:/gpsdataJson.sdf'"))
            {
                List<gpsDataJasonTable> datalist = context.gpsData.ToList();
                return datalist;
            }
        }

        public void delete()
        {
            using (DBJsonDataContext context = new DBJsonDataContext("Data Source='isostore:/gpsdataJson.sdf'"))
            {
                context.DeleteDatabase();
                context.CreateDatabase();
            }
        }

        public bool isEmpty()
        {
            using (DBJsonDataContext db = new DBJsonDataContext("Data Source='isostore:/gpsdataJson.sdf'"))
            {
                if (db.gpsData.Count() == 0)
                    return true;
            }
            return false;
        }

    
    }
}
