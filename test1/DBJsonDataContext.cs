using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test1
{
    class DBJsonDataContext : DataContext
    {
        public DBJsonDataContext(String connectionString)
            : base(connectionString)
        { }

        public Table<gpsDataJasonTable> gpsData
        {
            get
            {
                return this.GetTable<gpsDataJasonTable>();
            }
        }
    
    
    }
}
