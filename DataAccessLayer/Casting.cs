using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class Casting
    {
        public int ID { get; set; }
        public int PatternID { get; set; }
        public string Pattern { get; set; }
        public int ShiftID { get; set; }
        public string Shift { get; set; }
        public DateTime CastingDate { get; set; }
        public int ProductID { get; set; }
        public string Product { get; set; }
    }
}
