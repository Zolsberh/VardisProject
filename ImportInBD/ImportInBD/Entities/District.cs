using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImportInBD.Entities
{
    public class District : IEntityFromAccess
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Index{ get; set; }
    }
}
