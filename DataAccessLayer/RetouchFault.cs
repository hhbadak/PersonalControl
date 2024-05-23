using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class RetouchFault
    {
        public int ID { get; set; }
        public string Barcode { get; set; }
        public byte RetouchFaultID { get; set; }
        public string RetouchFaultStr { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionDateStr { get; set; }
        public byte PersonalInfo { get; set; }
    }
}
