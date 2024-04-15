using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace ImportInBD.ContolEntities
{
    internal class MaterialFromDBF
    {

        public string? CODE { get; set; }
        public string? NAME { get; set; }
        public string? EDIZ { get; set; }
        public double KOLORDER { get; set; }
    }
}
