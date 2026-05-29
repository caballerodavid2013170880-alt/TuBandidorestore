using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.PayPal.Pago;
using SUVAN.BackOffice.Models.PayPal.Token;


namespace SUVAN.BackOffice.Service.PayPal
{
    public interface IPayPalService
    {
        Task<PayPalAccessTokenResponse> GenerarTokenPayPal(int userId, PayPalPeticionGeneracionToken pmtPeticion);
        PayPalPeticionGeneracionToken ArmaObjetoRequestTokenPayPal(string grant_type, int userId = 0);

        Task<CreateOrderResponse> CreateOrder(string token, CreateOrderRequest pmtPeticion);
        CreateOrderRequest ArmaObjetoRequestCreateOrder(string gOrdenId, string monto);

        Task<OrdersCaptureResponse> OrdersCapture(string token, string order_id);


    }
}
