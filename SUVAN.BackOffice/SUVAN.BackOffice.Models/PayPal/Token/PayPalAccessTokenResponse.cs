using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SUVAN.BackOffice.Models.PayPal.Token
{
    public class PayPalAccessTokenResponse
    {
        private string codigoError { get; set; }
        private string error { get; set; }

        private string scope { get; set; }
        private string access_token { get; set; }
        private string token_type { get; set; }
        private string app_id { get; set; }
        private int expires_in { get; set; }
        private string nonce { get; set; }

        public string CodigoError { get { return codigoError; } }
        public string Error { get { return error; } }
        public string Scope { get { return scope; } }
        public string Access_token { get { return access_token; } }
        public string Token_type { get { return token_type; } }
        public string App_id { get { return app_id; } }
        public int  Expires_in { get { return expires_in; } }
        public string Nonce { get { return nonce; } }


        public void generar(string dataCodigoError, string dataError, string dataScope, string dataAccess_token, string dataToken_type, string dataApp_id, int dataExpires_in, string dataNonce)
        {
            codigoError = dataCodigoError;
            error = dataError;
            scope = dataScope;
            access_token = dataAccess_token;
            token_type = dataToken_type;
            app_id = dataApp_id;
            expires_in = dataExpires_in;
            nonce = dataNonce;
        }
    }
}
