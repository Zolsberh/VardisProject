using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportInBD
{
    public class Constructor: IEntityFromAccess
    {
        public int ConstructorID { get; set; }
        public string? ConstructorName { get; set; }
    }
}
