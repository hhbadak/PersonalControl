using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class GlazingFault
    {
        public int ID { get; set; }
        public int CodeID { get; set; }
        public string Code { get; set; }
        public byte ColorID { get; set; }
        public string Color { get; set; }
        public int FaultID { get; set; }
        public string Fault { get; set; }
        public int EmployeeID { get; set; }
        public string Employee { get; set; }
        public string Barcode { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get; set; }
        public int ShiftID { get; set; }
        public string Shift { get; set; }
    }
}
