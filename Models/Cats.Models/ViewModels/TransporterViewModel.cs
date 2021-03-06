﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cats.Models.ViewModels
{
    public class TransporterViewModel
    {
        public int TransporterID { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        public int Region { get; set; }
        public string RegioName { get; set; }
        public string ZoneName { get; set; }
        [Display(Name = "Sub City")]
        public string SubCity { get; set; }
        public int Zone { get; set; }
        public string TelephoneNo { get; set; }
        public string MobileNo { get; set; }
        public decimal Capital { get; set; }
        public string BidNo { get; set; }
        public string Fdp { get; set; }
        public string Source { get; set; }
        public string Desitnation { get; set; }
        
       
    }
}
