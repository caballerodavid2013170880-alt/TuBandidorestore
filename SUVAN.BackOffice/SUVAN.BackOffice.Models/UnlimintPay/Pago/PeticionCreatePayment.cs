using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.UnlimintPay.Pago
{
    public class PeticionCreatePayment
    {
        public Request request { get; set; }
        public Merchant_order merchant_order { get; set; }
        public Payment_data payment_data { get; set; }
        public Customer customer { get; set; }
        public string payment_method { get; set; }
        public Return_urls return_urls { get; set; }
        public PeticionCreatePayment(PeticionGeneracionPago peticion)
        {
            this.request = new Request();
            this.request.time = peticion.request.time;
            this.request.id = peticion.request.id;
            this.merchant_order = new Merchant_order();
            this.merchant_order.id = peticion.merchant_order.id;
            this.merchant_order.description = peticion.merchant_order.description;
            this.payment_data = new Payment_data();
            this.payment_data.amount = peticion.payment_Data.amount;
            this.payment_data.currency = peticion.payment_Data.currency;
            this.customer = new Customer();
            this.customer.email = peticion.customer.email;
            this.customer.id = peticion.customer.id;
            this.customer.locale = peticion.customer.locale;
            this.payment_method = peticion.payment_method;
            this.return_urls = new Return_urls();
            this.return_urls.success_url = peticion.return_urls.success_url;
            this.return_urls.inprocess_url = peticion.return_urls.inprocess_url;
            this.return_urls.decline_url = peticion.return_urls.decline_url;
            this.return_urls.cancel_url = peticion.return_urls.cancel_url;
        }
        private PeticionCreatePayment()
        {

        }
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
