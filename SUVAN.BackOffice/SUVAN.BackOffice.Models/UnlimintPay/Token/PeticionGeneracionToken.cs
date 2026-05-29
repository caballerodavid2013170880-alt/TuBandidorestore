using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.UnlimintPay.Token
{
    public class PeticionGeneracionToken
    {
        public string endPoint { get; set; }
        public string grant_type { get; set; }
        public string terminal_code { get; set; }
        public string password { get; set; }
        public string refresh_token { get; set; }
        public string token { get; set; }
    }
}
