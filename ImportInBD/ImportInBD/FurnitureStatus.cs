using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportInBD
{
    public class FurnitureStatus: IEntityFromAccess
    {
        public int FurnitureStatusID { get; set; }
        public string FurnitureStatusName { get; set; }
    }
}
