﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cats.Models
{
    
        public partial class Ration
        {
            public int RationID { get; set; }
            public int CommodityID { get; set; }
            public decimal Amount { get; set; }
        }
  
}
