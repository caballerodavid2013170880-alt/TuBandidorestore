using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Models.Ubicacion;

namespace SUVAN.BackOffice.Service.Monitoreo
{
    public interface IRastreoService
    {
        Task<List<CorridaAsignacion>> GetCorridas(int pEmpresa);

        Task<Rutum> GetRuta(int Idruta);

        Task<Paradum> GetParada(int IdParada);

        Task<SuVanResponse<RegistraUbicacionResponse>> SetUbicacion(RegistraUbicacionRequest data);

        Task<SuVanResponse<UbicacionResponse>> GetUbicacion(int idCorridaAsignacion);
    }
}
