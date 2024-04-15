using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportInBD.Entities
{
    public class Contractor : IEntityFromAccess
    {
        public int ContractorID { get; set; }
        public string? ContractorName { get; set; }
    }
}
