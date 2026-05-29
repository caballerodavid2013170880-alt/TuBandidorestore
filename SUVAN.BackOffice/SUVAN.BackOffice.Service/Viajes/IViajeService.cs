using SUVAN.BackOffice.Models.Viajes;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Service.Viajes
{
  public interface IViajeService
  {
    Task<SuVanResponse<List<ViajeServicioResponse>>> BuscaServicio(int userId, string emailUser, ViajeServicioRequest data);

    Task<SuVanResponse<List<ViajeDisponibilidadResponse>>> BuscaDisponibilidad(int userId, string emailUser, ViajeDisponibilidadRequest data);

    Task<SuVanResponse<ReservacionViajeReponse>> ApartaReservacion(ReservacionViajeRequest data, int userId);

    Task<SuVanResponse<BoletoViajeResponse>> ObtenBoleto(BoletoViajeRequest data);

    Task<SuVanResponse<RecorridoViajeResponse>> ObtenRecorrido(RecorridoViajeRequest data);

    Task<SuVanResponse<List<ViajeRutaModels>>> ViajesFrecuentes(int userId);

    Task<SuVanResponse<List<ViajeRutaResponse>>> ViajeCurso(int userId, int numViajes);

    Task<SuVanResponse<List<ViajeRutaResponse>>> ProximosViajes(int userId, int numViajes);

    Task<SuVanResponse<List<ViajeRutaResponse>>> ViajesAnteriores(int userId);

    Task<ObtenDatosDeViajePorIdDeVajeModel> ObtenRutaViajePorId(int ViajeId);

    Task<SuVanResponse<RecorridoViajeResponse>> ObtenRecorridoOperador(RecorridoViajeOperadorRequest data);

    Task<Viaje> ObtenInfoViaje(int ViajeId);

    Task<SuVanResponse<CancelacionViaje>> Cancelacion(int userId, CancelaViajeRequest data);
    Task<SuVanResponse<List<ViajesServicioFechaResponse>>> BuscaFechasRuta(ViajeFechasRequest data);
    Task<SuVanResponse<List<BoletoViajeResponse>>> ObtenBoletoOffline(int userId);
  }
}
