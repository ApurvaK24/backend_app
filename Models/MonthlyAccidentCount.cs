using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_app.Models
{
    public class MonthlyAccidentCount
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int AccidentCount { get; set; }
    }
}