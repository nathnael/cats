﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cats.Areas.Procurement.Models
{
    public class BidDetailsViewModel
    {
        public int BidDetailID { get; set; }
        public int BidID { get; set; }
        public string Region { get; set; }
        public decimal AmountForReliefProgram { get; set; }
        public decimal AmountForPSNPProgram { get; set; }
        public decimal BidDocumentPrice { get; set; }
        public decimal CPO { get; set; }
        public BidDetailEdit Edit { get; set; }

        public class BidDetailEdit
        {
            public int Number { get; set; }
            public decimal AmountForReliefProgram { get; set; }
            public decimal AmountForPSNPProgram { get; set; }
            public decimal BidDocumentPrice { get; set; }
            public decimal CPO { get; set; }
            public decimal AmountForReliefProgramPlanned { get; set; }
            public decimal AmountForPSNPProgramPlanned { get; set; }

        }

        
    }
}