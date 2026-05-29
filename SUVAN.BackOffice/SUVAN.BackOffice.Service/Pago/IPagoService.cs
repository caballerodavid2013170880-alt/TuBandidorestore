
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Models.UnlimintPay.Token;
using SUVAN.BackOffice.Service.UnlimintPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Pago
{
    public interface IPagoService
    {

        #region Metodos para Pago con "Tarjeta de Credito o Debito"
        Task<SuVanResponse<PagoResponse>> GenerarPago(int userId, PagoRequest data, string emailUser);

        Task<SuVanResponse<ConfirmarPagoResponse>> ConfirmaPago(int userId, ConfirmarPagoRequest data);

        #endregion
        Task<bool> GuardaBitacoraTransaccion(string respuesta);

        #region Metodos para Pago con "Monedero"
        Task<SuVanResponse<PagoMonederoResponse>> GenerarPagoMonedero(int userId,string emailUser, PagoMonederoRequest data, string code);
        #endregion

        Task<Transaccion> GuardaTransaccion(int userId, int MetodoPagoId, int TipoTransacionId, decimal Monto, string? Numeroordenpay, string? Numeropeticionpay, string? CodigoOTP, DateTime? Codigoexp);

        #region  Metodos para Pago con "PayPal"
        Task<SuVanResponse<PagoResponse>> GenerarPagoConPayPal(int userId, string emailUser, PagoRequest data);
        Task<SuVanResponse<ConfirmarPagoResponse>> ConfirmaPagoPayPal(int userId, ConfirmarPagoRequest data);
        #endregion

        Task<Transaccion?> ObtenInfoTransaccionOrdenPeticion(string OrdenId, string PeticionId);


        Task<decimal> ObtenTarifa(int userId, string emailUser, int rutaId, int? corridaId, int estacionAbordajeId, int estacionDescensoId, string codigoDescuento, bool aplicaPromocion = true);

        Task<decimal> AplicaPromocion(decimal tarifa, int userId, string emailUser, int rutaId, int? corridaId, string codigoDescuento);

        Task<bool> ActualizaEstatusTransaccion(int TransaccionId, int EstatusTransaccionId);

        Task ActualizaOcupacion(int reservacionId, bool cancelaOcupacion = false);
    }
}
