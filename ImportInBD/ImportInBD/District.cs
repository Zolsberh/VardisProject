﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportInBD
{
    public class District : IEntityFromAccess
    {

        public int? Id { get; set; }
        public string? Name { get; set; }          
    }
}