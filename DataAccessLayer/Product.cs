using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class Product : QualityData
    {
        public int ID { get; set; }
        public string Barcode { get; set; }
        public int CastingPersonalID { get; set; }
        public string CastingPersonal { get; set; }
        public byte Piece { get; set; }
        public string Statu { get; set; }
        public DateTime CastingDate { get; set; }
        public string CastingDateStr { get; set; }
        public int BenchMoldID { get; set; }
        public int StandID { get; set; }
        public byte FaultID { get; set; }
        public string Fault { get; set; }
        public byte ShiftID { get; set; }
        public string Shift { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionDateStr { get; set; }
        public int DailyCastingID { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }
        public Int16 Quality { get; set; }
        public int Count { get; set; }
        public string Code { get; set; }
        public string RetouchFire { get; set; }
    }
}
