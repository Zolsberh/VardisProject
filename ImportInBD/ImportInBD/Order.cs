using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportInBD
{
    public class Order:IEntityFromAccess
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DeadLine { get; set; } = DateTime.Now;  
        public Manager Manager { get; set; }
        public Contractor Contractor { get; set; }
        public Constructor Constructor { get; set; }
        public DateTime CloseOrder { get; set; }
        public Status Status { get; set; }
    }
}
