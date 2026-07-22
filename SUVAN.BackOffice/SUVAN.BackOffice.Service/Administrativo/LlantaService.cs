using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class LlantaService : ILlantaService
    {
        private readonly SuvanDbContext context;

        public LlantaService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<LlantaViewModel>> GetLlantas(int idEmpresa)
        {
            var query = context.Llanta
                .AsNoTracking()
                .Where(x => !x.Eliminado);

            if (idEmpresa > 0)
            {
                query = query.Where(x => x.IdEmpresa == (uint)idEmpresa);
            }

            return await (
                    from llanta in query
                    join deposito in context.Depositos.AsNoTracking()
                        on llanta.IdDeposito equals (uint?)deposito.IdDeposito into depositoGroup
                    from deposito in depositoGroup.DefaultIfEmpty()
                    orderby llanta.CodigoLlanta
                    select new LlantaViewModel
                    {
                        IdLlanta = llanta.IdLlanta,
                        CodigoLlanta = llanta.CodigoLlanta,
                        NumeroSerieDot = llanta.NumeroSerieDot,
                        Estado = llanta.IdEstadoLlantaNavigation.Nombre,
                        Deposito = deposito != null ? deposito.NombreDeposito : null,
                        PresionMinimaPsi = llanta.PresionMinimaPsi,
                        PresionMaximaPsi = llanta.PresionMaximaPsi,
                        ProfundidadOriginalMm = llanta.ProfundidadOriginalMm,
                        FechaAdquisicion = llanta.FechaAdquisicion,
                        CostoAdquisicion = llanta.CostoAdquisicion
                    })
                .ToListAsync();
        }
    }
}
