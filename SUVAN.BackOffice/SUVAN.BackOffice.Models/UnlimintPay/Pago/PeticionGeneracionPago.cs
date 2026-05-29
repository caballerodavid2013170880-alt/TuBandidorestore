using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.UnlimintPay.Pago
{
    public class PeticionGeneracionPago
    {
        public string endPoint { get; set; }
        public string token { get; set; }
        public Request request { get; set; }
        public Merchant_order merchant_order { get; set; }
        public Payment_data payment_Data { get; set; }
        public Customer customer { get; set; }
        public string payment_method { get; set; }
        public Return_urls return_urls { get; set; }
        public class Request
        {
            public string id { get; set; }
            public string time { get; set; }
        }
        public class Merchant_order
        {
            public string id { get; set; }
            public string description { get; set; }
        }
        public class Payment_data
        {
            public string amount { get; set; }
            public string currency { get; set; }
        }
        public class Customer
        {
            public string email { get; set; }
            public string id { get; set; }
            public string locale { get; set; }
        }
        public class Return_urls
        {
            public string cancel_url { get; set; }
            public string decline_url { get; set; }
            public string inprocess_url { get; set; }
            public string success_url { get; set; }
        }
    }
}