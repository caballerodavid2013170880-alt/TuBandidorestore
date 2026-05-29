using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Ubicacion;

namespace SUVAN.BackOffice.Service.Monitoreo
{
    public class RastreoService : IRastreoService
    {
        private readonly SuvanDbContext context;

        public RastreoService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene las corridas que estan desplegadas en viaje
        /// </summary>
        /// <returns></returns>
        public async Task<List<CorridaAsignacion>> GetCorridas(int pEmpresa)
        {
            DateTime vHoy = DateTime.Today;

            var corridas = await context.CorridaAsignacions
                .Include(x => x.ConductorIdconductorNavigation)
                .Include(x => x.VehiculoIdvehiculoNavigation)
                .Include(x => x.CorridaIdcorridaNavigation)
                .Where(x => x.Fecha == vHoy & x.EstatusviajeIdestatusviaje == 2 & x.CorridaIdcorridaNavigation.EmpresaIdempresa == pEmpresa).ToListAsync();

            return corridas!;
        }

        public async Task<Rutum> GetRuta(int pIdruta)
        {
            var ruta = await context.Ruta
                .Include(x => x.RutaParada)
                .ThenInclude(y => y.ParadaIdparadaNavigation)
                .Where(z => z.Idruta == pIdruta)
                .FirstOrDefaultAsync();

            return ruta!;
        }

        public async Task<Paradum> GetParada(int pIdParada)
        {
            var parada = await context.Parada
                .Where(z => z.Idparada == pIdParada)
                .FirstOrDefaultAsync();

            return parada!;
        }

        public async Task<SuVanResponse<RegistraUbicacionResponse>> SetUbicacion(RegistraUbicacionRequest pData)
        {
            SuVanResponse<RegistraUbicacionResponse> response = new();

            var vCorridaAsignacion = await context.CorridaAsignacions
                .Where(z => z.IdcorridaAsignacion == pData.IdcorridaAsignacion)
                .FirstOrDefaultAsync();

            if (vCorridaAsignacion != null)
            {
                vCorridaAsignacion.CurrentLat = pData.Latitude;
                vCorridaAsignacion.CurrentLong = pData.Longitude;
                context.SaveChanges();

                response.Mensaje = "Solicitud exitosa";
                response.CodigoMensaje = "200";
            }
            else
            {
                response.Mensaje = "Corrida no Encontrada";
                response.CodigoMensaje = "200";
            }

            return response;
        }

        public async Task<SuVanResponse<UbicacionResponse>> GetUbicacion(int idCorridaAsignacion)
        {
            UbicacionResponse? vUbicacionResponse = new UbicacionResponse();
            SuVanResponse<UbicacionResponse> response = new();

            var vCorridaAsignacion = await context.CorridaAsignacions
                .Where(z => z.IdcorridaAsignacion == idCorridaAsignacion)
                .FirstOrDefaultAsync();

            if (vCorridaAsignacion != null)
            {
                vUbicacionResponse.IdcorridaAsignacion = idCorridaAsignacion;
                vUbicacionResponse.Latitude = vCorridaAsignacion.CurrentLat ?? 0;
                vUbicacionResponse.Longitude = vCorridaAsignacion.CurrentLong ?? 0;

                response.Mensaje = "Solicitud exitosa";
                response.CodigoMensaje = "200";
                response.Data = vUbicacionResponse;
            }

            return response;
        }
    }

}

