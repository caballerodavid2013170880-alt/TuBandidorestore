using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.PayPal.Token
{
    public class PayPalPeticionGeneracionToken
    {
        public string endPoint { get; set; }

        public string endPointGeneraTokenPayPal { get; set; }

        public string grant_type { get; set; }

        public string response_type { get; set; }

        public string refresh_token { get; set; }
        public string token { get; set; }

        public string client_id { get; set; }

        public string client_secret { get; set; }


    }
}
