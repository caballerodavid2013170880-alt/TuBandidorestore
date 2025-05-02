/*
 *  Ejemplo del JSON requets de acuerdo  a la documentación de PayPal
    https://developer.paypal.com/docs/api/orders/v2/#orders_create
 {
    "id": "9UK01520KA6742936",
    "intent": "CAPTURE",
    "status": "CREATED",
    "purchase_units": [
        {
            "reference_id": "default",
            "amount": {
                "currency_code": "MXN",
                "value": "10.00",
                "breakdown": {
                    "item_total": {
                        "currency_code": "MXN",
                        "value": "10.00"
                    }
                }
            },
            "payee": {
                "email_address": "john_merchant@example.com",
                "merchant_id": "C7CYMKZDG8D6E"
            },
            "items": [
                {
                    "name": "T-Shirt",
                    "unit_amount": {
                        "currency_code": "MXN",
                        "value": "10.00"
                    },
                    "quantity": "1",
                    "description": "Green XL"
                }
            ]
        }
    ],
    "create_time": "2024-01-20T06:16:38Z",
    "links": [
        {
            "href": "https://api.sandbox.paypal.com/v2/checkout/orders/9UK01520KA6742936",
            "rel": "self",
            "method": "GET"
        },
        {
            "href": "https://www.sandbox.paypal.com/checkoutnow?token=9UK01520KA6742936",
            "rel": "approve",
            "method": "GET"
        },
        {
            "href": "https://api.sandbox.paypal.com/v2/checkout/orders/9UK01520KA6742936",
            "rel": "update",
            "method": "PATCH"
        },
        {
            "href": "https://api.sandbox.paypal.com/v2/checkout/orders/9UK01520KA6742936/capture",
            "rel": "capture",
            "method": "POST"
        }
    ]
}
 */

using SUVAN.BackOffice.Models.UnlimintPay.Pago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.PayPal.Pago
{
    public class CreateOrderResponse
    {
        //privadas
        private string _codigoError { get; set; }
        private string _error { get; set; }
        private CreateOrderResponseWS _datosOrder { get; set; }


        //publicas
        public string CodigoError { get { return _codigoError; } }
        public string Error { get { return _error; } }
        public CreateOrderResponseWS DatosOrder { get { return _datosOrder; } }


        public void generar(string pmtCodigoError, string pmtError, CreateOrderResponseWS pmtRespuesta = null)
        {
            this._codigoError = pmtCodigoError;
            this._error = pmtError;
            if (pmtRespuesta != null)
            {
                this._datosOrder = pmtRespuesta;
            }
        }
    }

    public class CreateOrderResponseWS
    {
        public class Amount
        {
            public string currency_code { get; set; }
            public string value { get; set; }
            public Breakdown breakdown { get; set; }
        }

        public class Breakdown
        {
            public ItemTotal item_total { get; set; }
        }

        public class Item
        {
            public string name { get; set; }
            public UnitAmount unit_amount { get; set; }
            public string quantity { get; set; }
            public string description { get; set; }
        }

        public class ItemTotal
        {
            public string currency_code { get; set; }
            public string value { get; set; }
        }

        public class Link
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }

        public class Payee
        {
            public string email_address { get; set; }
            public string merchant_id { get; set; }
        }

        public class PurchaseUnit
        {
            public string reference_id { get; set; }
            public Amount amount { get; set; }
            public Payee payee { get; set; }
            public List<Item> items { get; set; }
        }


        public string id { get; set; }
        public string intent { get; set; }
        public string status { get; set; }
        public List<PurchaseUnit> purchase_units { get; set; }
        public DateTime create_time { get; set; }
        public List<Link> links { get; set; }


        public class UnitAmount
        {
            public string currency_code { get; set; }
            public string value { get; set; }
        }

    }
}
