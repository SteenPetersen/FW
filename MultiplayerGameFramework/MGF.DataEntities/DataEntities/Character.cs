﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// video 22

namespace MGF.DataEntities
{
    public class Character
    {

        public int Id { get; set; }
        public String Name { get; set; }

        public ICollection<Stat> Stats { get; set; }
    }
}
