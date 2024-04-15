using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImportInBD.Entities;

namespace ImportInBD.ContolEntities
{
    public class ControlOrderMaterial
    {
        public string? FromDBFID { get; set; }
        public int? FromAccessID { get; set; }
        public string? NameFromDBF { get; set; }
        public string? NameFromAccess { get; set; }
        public string? UnitFromDBF { get; set; }
        public Unit? UnitFromAccess { get; set; }
        public double Count { get; set; }
    }
}
