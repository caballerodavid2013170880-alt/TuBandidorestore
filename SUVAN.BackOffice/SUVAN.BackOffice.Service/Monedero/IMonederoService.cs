using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Monedero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Monedero
{
    public interface IMonederoService
    {

        Task<SuVanResponse<MonederoSaldoResponse>> ObtenMonedero(int userId);

        #region Metodos para Recarga de Monedero con "Tarjeta de Credito o Debito"
        Task<SuVanResponse<RecargaMonederoResponse>> RecargaMonederoConTarjeta(int userId, RecargaMonederoRequest data, string emailUser);

        Task<SuVanResponse<ConfirmaRecargaMonederoResponse>> ConfirmaRecargaMonederoConTarjeta(int userId, ConfirmaRecargaMonederoRequest data);
        #endregion

        #region Metodos para Recarga de Monedero con "PayPal"
        Task<SuVanResponse<RecargaMonederoResponse>> RecargaMonederoConPayPal(int userId, RecargaMonederoRequest data, string emailUser);

        Task<SuVanResponse<ConfirmaRecargaMonederoResponse>> ConfirmaRecargaMonederoConPayPal(int userId, ConfirmaRecargaMonederoRequest data);
        #endregion

        Task<decimal?> ActualizaSaldoMonedero(int userId, string OrdenId, string PeticionId, int EstatusPago);

    }
}
