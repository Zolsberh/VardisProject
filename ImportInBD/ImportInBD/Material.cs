using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportInBD
{
    public class Material:IEntityFromAccess
    {
        public int MaterialID { get; set; }
        public string? MaterialName { get; set; }
        public Unit? MaterialUnit { get; set;}
        public double MaterialCount { get; set; }
    }
}
