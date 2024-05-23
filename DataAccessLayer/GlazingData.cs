using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class GlazingData
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public string Product { get; set; }
        public byte GlazingPersonalID { get; set; }
        public string GlazingPersonal { get; set; }
        public DateTime GlazingDate { get; set; }
        public string GlazingDateStr { get; set; }
        public string Barcode { get; set; }
        public byte ColorID { get; set; }
        public string Color { get; set; }
    }
}
