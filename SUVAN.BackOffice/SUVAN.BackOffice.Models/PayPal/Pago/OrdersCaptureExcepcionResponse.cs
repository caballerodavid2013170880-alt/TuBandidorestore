using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.PayPal.Pago
{
    public class OrdersCaptureExcepcionResponse
    {

        public string name { get; set; }
        public List<Detail> details { get; set; }
        public string message { get; set; }
        public string debug_id { get; set; }
        public List<Link> links { get; set; }

        public class Detail
        {
            public string issue { get; set; }
            public string description { get; set; }
        }

        public class Link
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }

    }
}
