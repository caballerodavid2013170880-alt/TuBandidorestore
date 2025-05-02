/*
 Ejemplo del JSON requets de acuerdo  a la documentación de PayPal
https://developer.paypal.com/docs/api/orders/v2/#orders_create
{
    "intent": "{{order_intent}}",
    "purchase_units": [{
            "items": [{
                    "name": "T-Shirt",
                    "description": "Green XL",
                    "quantity": "1",
                    "unit_amount": {
                        "currency_code": "MXN",
                        "value": "10.00"
                    }
                }
            ],
            "amount": {
                "currency_code": "MXN",
                "value": "10.00",
                "breakdown": {
                    "item_total": {
                        "currency_code": "MXN",
                        "value": "10.00"
                    }
                }
            }
        }
    ],
    "application_context": {
        "return_url": "https://example.com/return",
        "cancel_url": "https://example.com/cancel"
    }
}
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.PayPal.Pago
{
    public class CreateOrderRequest
    {
        public string intent { get; set; }
        public List<PurchaseUnit> purchase_units { get; set; }
        public ApplicationContext application_context { get; set; }

        public class Amount
        {
            public string currency_code { get; set; }
            public string value { get; set; }
            public Breakdown breakdown { get; set; }
        }

        public class ApplicationContext
        {
            public string return_url { get; set; }
            public string cancel_url { get; set; }
        }

        public class Breakdown
        {
            public ItemTotal item_total { get; set; }
        }

        public class Item
        {
            public string name { get; set; }
            public string description { get; set; }
            public string quantity { get; set; }
            public UnitAmount unit_amount { get; set; }
        }

        public class ItemTotal
        {
            public string currency_code { get; set; }
            public string value { get; set; }
        }

        public class PurchaseUnit
        {
            public List<Item> items { get; set; }
            public Amount amount { get; set; }
        }

      
        public class UnitAmount
        {
            public string currency_code { get; set; }
            public string value { get; set; }
        }

    }
}
