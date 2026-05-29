using Microsoft.Extensions.Options;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Membresia;
using SUVAN.BackOffice.Models.Monedero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Membresia
{
    public interface IMembresiaService 
    {
        #region  Metodos para Pago de Memebresia con "Tarjeta de Credito o Debito"
        Task<SuVanResponse<PagaMembresiaConTarjetaResponse>> PagaMembresiaConTarjeta(int userId, PagaMembresiaConTarjetaRequest data, string emailUser);

        Task<SuVanResponse<ConfirmaPagoMembresiaConTarjetaResponse>> ConfirmaPagoMembresiaConTarjeta(int userId, ConfirmaPagoMembresiaConTarjetaRequest data);
        #endregion

        #region Metodos para Pago de Memebresia con "Monedero"
        Task<SuVanResponse<PagaMembresiaConMonederoResponse>> PagaMembresiaConMonedero(int userId, PagaMembresiaConMonederoRequest data, string emailUser, string code);
        #endregion

        #region  Metodos para Pago de Membresia con "PayPal"
        Task<SuVanResponse<PagaMembresiaConPayPal>> PagaMembresiaConPayPal(int userId, PagaMembresiaConPayPalRequest data, string emailUser);

        Task<SuVanResponse<ConfirmaPagoMembresiaConPayPalResponse>> ConfirmaPagoMembresiaConPayPal(int userId, ConfirmaPagoMembresiaConPayPalRequest data);
        #endregion
    }


}
