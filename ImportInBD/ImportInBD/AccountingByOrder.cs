using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportInBD
{
    public class AccountingByOrder
    {
       // private District? district;
        private Material? materialName;
        private int materialId;
        private int accountigByOrdersID;
        private string? ediz;
        private double ramainderRatio;

        public int AccountigByOrdersID { get => accountigByOrdersID; set => accountigByOrdersID = value; }
        public int MaterialId { get => materialId; set => materialId = value; }
        public Material? MaterialName { get => materialName; set => materialName = value; }
        public string? Ediz { get => ediz; set => ediz = value; }
        public double MaterialCount { get; set; }
        public District? District { get; set; }
        public double MaterialUtolizationFactor { get; set; } = 100;
        public double RamainderRatio { get => ramainderRatio; set => ramainderRatio = value; }

    }
}
