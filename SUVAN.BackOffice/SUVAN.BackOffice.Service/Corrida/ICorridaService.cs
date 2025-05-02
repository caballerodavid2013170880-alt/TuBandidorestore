using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Corrida;
using SUVAN.BackOffice.Models.ManejoRespuesta;

namespace SUVAN.BackOffice.Service.Corrida
{
    public interface ICorridaService
    {

        Task<SuVanResponse<CorridaEstacionesResponse>> Estaciones(int idCorrida);

        Task<SuVanResponse<CorridaParadaResponse>> ComenzarAbordaje(ComenzarAbordajeRequest data,int ConductorId);

        Task<SuVanResponse<CorridaParadaResponse>> RegresarAbordaje(AbordajeRequest data);

        Task<SuVanResponse<CorridaParadaResponse>> TerminarAbordaje(AbordajeRequest data);

        Task<SuVanResponse<CorridaParadaResponse>> CheckIn(BoletoCheckRequest data, int ConductorId);

    }
}
