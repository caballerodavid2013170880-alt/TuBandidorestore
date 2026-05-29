using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.UnlimintPay.Token
{
    public class RespuestaGeneracionToken
    {

        private string codigoError { get; set; }
        private string error { get; set; }
        private string tokenType { get; set; }
        private string accessToken { get; set; }
        private string refreshToken { get; set; }
        private int expiresIn { get; set; }
        private int refreshExpiresIn { get; set; }

        public string CodigoError { get { return codigoError; } }
        public string Error { get { return error; } }
        public string TokenType { get { return tokenType; } }
        public string AccessToken { get { return accessToken; } }
        public string RefreshToken { get { return refreshToken; } }
        public int ExpiresIn { get { return expiresIn; } }
        public int RefreshExpiresIn { get { return refreshExpiresIn; } }

        public void generar(string pmtCodigoError, string pmtError, string pmtTokenType, string pmtAccessToken, string pmtRefreshToken, int pmtExpiresIn, int pmtRefreshExpiresIn)
        {
            codigoError = pmtCodigoError;
            error = pmtError;
            tokenType = pmtTokenType;
            accessToken = pmtAccessToken;
            refreshToken = pmtRefreshToken;
            expiresIn = pmtExpiresIn;
            refreshExpiresIn = pmtRefreshExpiresIn;
        }

    }
}

