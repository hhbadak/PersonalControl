using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class QualityData
    {
        public int ID { get; set; }
        public string Barcode { get; set; }
        public int ProductCode { get; set; }
        public string ProductCodeStr { get; set; }
        public DateTime CastDate { get; set; }
        public string CastDateStr { get; set; }
        public int CastPersonalID { get; set; }
        public string CastPersonalStr { get; set; }
        public byte GlazingTerritory { get; set; }
        public byte QualityID { get; set; }
        public string QualityStr { get; set; }
        public byte FaultID { get; set; }
        public string Fault { get; set; }
        public DateTime ControlDate { get; set; }
        public string ControlDateStr { get; set; }
        public string ControlPersonal { get; set; }
        public byte Kiln { get; set; }
        public string KilnStr { get; set; }
        public byte Firing { get; set; }
        public string FiringStr { get; set; }
        public byte Color { get; set; }
        public string ColorStr { get; set; }
        public byte StockTerritory { get; set; }
        public string IsItTest { get; set; }
        public DateTime ControlTime { get; set; }
        public string ControlTimeStr { get; set; }
    }
}
