﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportInBD
{
    public class Manager:IEntityFromAccess
    {
        public int ManagerID { get; set; }
        public string? ManagerName { get; set; }
        public string? Phone { get; set; }
    }
}
