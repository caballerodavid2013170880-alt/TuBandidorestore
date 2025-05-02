using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.UnlimintPay.Pago
{
    public class RespuestaValidacionPagoWS
    {
        public List<Data> data { get; set; }
        public bool has_more { get; set; }

        public class Data
        {
            public string payment_method { get; set; }
            public Merchant_order merchant_order { get; set; }
            public Payment_data payment_data { get; set; }
            public Card_account card_account { get; set; }
        }
        public class Merchant_order
        {
            public string id { get; set; }
        }
        public class Payment_data
        {
            public string id { get; set; }
            public string status { get; set; }
            public decimal amount { get; set; }
            public string currency { get; set; }           
            public string created { get; set; }
            public string auth_code { get; set; }           
            public string note { get; set; }
            public bool is_3d { get; set; }

        }
        public class Card_account
        {
            public string masked_pan { get; set; }
            public string holder { get; set; }
            public string issuing_country_code { get; set; }

        }
        public class Customer
        {
            public string email { get; set; }
            public string ip { get; set; }
            public string locale { get; set; }
            public string phone { get; set; }

        }
    }
}
