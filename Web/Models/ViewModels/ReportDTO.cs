﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cats.Models.ViewModels
{
    public class ReportDTO
    {
        public byte[] RenderBytes { get; set; }
        public string    MimeType { get; set; }
    }
}