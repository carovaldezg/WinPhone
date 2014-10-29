using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;

namespace bd1
{
    class DBDataContext : DataContext
    {
        public DBDataContext(string connectionString) : base(connectionString)
        {
        }

        public Table<gpsData> gpsData
        {
            get
            {
                return this.GetTable<gpsData>();
            }
        }
    }
}
