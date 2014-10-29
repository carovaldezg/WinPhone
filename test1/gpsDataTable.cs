using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace bd1
{
    [Table(Name = "positionInTime")]
    class gpsData
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Latitude { get; set; }

        [Column(CanBeNull = false)]
        public string Longitude { get; set; }

        [Column(CanBeNull = false)]
        public string Timestamp { get; set; }



    }
}
