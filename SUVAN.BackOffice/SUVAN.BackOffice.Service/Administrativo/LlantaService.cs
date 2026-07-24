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
                        FechaAdquisicion = llanta.FechaAdquisicion,
                        CostoAdquisicion = llanta.CostoAdquisicion
                    })
                .ToListAsync();
        }

        public async Task<LlantaCrearViewModel> GetCrearViewModel(int idEmpresa, string nombreEmpresa, LlantaCrearViewModel? model = null)
        {
            var vRet = model ?? new LlantaCrearViewModel();
            vRet.IdEmpresa = idEmpresa;
            vRet.NombreEmpresa = nombreEmpresa;

            vRet.Regiones = await context.Regions
                .AsNoTracking()
                .Where(r => r.IdEmpresa == idEmpresa)
                .OrderBy(r => r.NombreRegion)
                .Select(r => new LlantaCrearViewModel.CatalogItemViewModel
                {
                    Id = r.IdRegion,
                    Nombre = r.NombreRegion
                })
                .ToListAsync();

            if (vRet.IdRegion > 0)
            {
                vRet.Plantas = await context.Planta
                    .AsNoTracking()
                    .Where(p => p.IdEmpresa == idEmpresa && p.IdRegion == vRet.IdRegion)
                    .OrderBy(p => p.NombrePlanta)
                    .Select(p => new LlantaCrearViewModel.CatalogItemViewModel
                    {
                        Id = p.IdPlanta,
                        Nombre = p.NombrePlanta
                    })
                    .ToListAsync();
            }

            if (vRet.IdPlanta > 0)
            {
                vRet.Zonas = await context.Zonas
                    .AsNoTracking()
                    .Where(z => z.IdEmpresa == idEmpresa && z.IdRegion == vRet.IdRegion && z.IdPlanta == vRet.IdPlanta)
                    .OrderBy(z => z.NombreZona)
                    .Select(z => new LlantaCrearViewModel.CatalogItemViewModel
                    {
                        Id = z.IdZona,
                        Nombre = z.NombreZona
                    })
                    .ToListAsync();
            }

            if (vRet.IdZona > 0)
            {
                vRet.Depositos = await context.Depositos
                    .AsNoTracking()
                    .Where(d => d.IdEmpresa == idEmpresa && d.IdRegion == vRet.IdRegion && d.IdPlanta == vRet.IdPlanta && d.IdZona == vRet.IdZona)
                    .OrderBy(d => d.NombreDeposito)
                    .Select(d => new LlantaCrearViewModel.CatalogItemViewModel
                    {
                        Id = d.IdDeposito,
                        Nombre = d.NombreDeposito
                    })
                    .ToListAsync();
            }

            return vRet;
        }

        public async Task<bool> CrearLlanta(LlantaCrearViewModel model, int idEmpresa, int idUsuario)
        {
            ValidarDatosCaptura(model);

            bool codigoDuplicado = await context.Llanta
                .AnyAsync(x => !x.Eliminado && x.CodigoLlanta.Trim().ToLower() == model.CodigoLlanta.Trim().ToLower());
            if (codigoDuplicado)
                throw new Exception("Ya existe una llanta con el mismo código.");

            bool serieDuplicada = await context.Llanta
                .AnyAsync(x => !x.Eliminado && x.NumeroSerieDot.Trim().ToLower() == model.NumeroSerieDot.Trim().ToLower());
            if (serieDuplicada)
                throw new Exception("Ya existe una llanta con el mismo número de serie / DOT.");

            await ValidarJerarquia(model, idEmpresa);

            var estadoEnDeposito = await context.LlantaEstados
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Nombre.ToLower() == "en depósito" || x.Nombre.ToLower() == "en deposito");

            var llanta = new Llantum
            {
                CodigoLlanta = model.CodigoLlanta.Trim(),
                NumeroSerieDot = model.NumeroSerieDot.Trim(),
                IdEstadoLlanta = estadoEnDeposito?.IdEstadoLlanta ?? (ushort)1,
                IdEmpresa = (uint)idEmpresa,
                IdRegion = (uint)model.IdRegion,
                IdPlanta = (uint)model.IdPlanta,
                IdZona = (uint)model.IdZona,
                IdDeposito = (uint)model.IdDeposito,                                
                FechaFabricacion = model.FechaFabricacion.HasValue ? DateOnly.FromDateTime(model.FechaFabricacion.Value) : null,                                
                CostoAdquisicion = model.CostoAdquisicion,
                FechaAdquisicion = DateOnly.FromDateTime(model.FechaAdquisicion!.Value),
                Observaciones = string.IsNullOrWhiteSpace(model.Observaciones) ? null : model.Observaciones.Trim(),
                Eliminado = false,
                FechaEliminacion = null,
                EliminadoPor = null,
                MotivoEliminacion = null,
                FechaCreacion = DateTime.Now,
                CreadoPor = (uint)idUsuario,
                FechaModificacion = null,
                ModificadoPor = null
            };

            context.Llanta.Add(llanta);
            await context.SaveChangesAsync();
            return true;
        }

        private static void ValidarDatosCaptura(LlantaCrearViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CodigoLlanta) || model.CodigoLlanta.Length > 30)
                throw new Exception("El código de llanta es obligatorio y no debe exceder 30 caracteres.");

            if (string.IsNullOrWhiteSpace(model.NumeroSerieDot) || model.NumeroSerieDot.Length > 30)
                throw new Exception("El número de serie / DOT es obligatorio y no debe exceder 30 caracteres.");

            if (model.PresionMinimaPsi.HasValue && model.PresionMinimaPsi < 0)
                throw new Exception("La presión mínima no puede ser negativa.");

            if (model.PresionMaximaPsi.HasValue && model.PresionMaximaPsi < 0)
                throw new Exception("La presión máxima no puede ser negativa.");

            if (model.PresionMinimaPsi.HasValue && model.PresionMaximaPsi.HasValue && model.PresionMaximaPsi < model.PresionMinimaPsi)
                throw new Exception("La presión máxima no puede ser menor que la presión mínima.");

            if (model.ProfundidadOriginalMm.HasValue && model.ProfundidadOriginalMm < 0)
                throw new Exception("La profundidad original no puede ser negativa.");

            if (model.VidaUtilEstimadaKm.HasValue && model.VidaUtilEstimadaKm < 0)
                throw new Exception("La vida útil estimada no puede ser negativa.");

            if (!model.CostoAdquisicion.HasValue || model.CostoAdquisicion < 0)
                throw new Exception("El costo de adquisición es obligatorio y no puede ser negativo.");

            if (!model.FechaAdquisicion.HasValue)
                throw new Exception("La fecha de adquisición es obligatoria.");

            if (model.Observaciones?.Length > 500)
                throw new Exception("Las observaciones no deben exceder 500 caracteres.");

            var hoy = DateTime.Today;
            if (model.FechaFabricacion.HasValue && model.FechaFabricacion.Value.Date > hoy)
                throw new Exception("La fecha de fabricación no puede ser posterior a la fecha actual.");

            if (model.FechaAdquisicion.HasValue && model.FechaAdquisicion.Value.Date > hoy)
                throw new Exception("La fecha de adquisición no puede ser posterior a la fecha actual.");
        }

        private async Task ValidarJerarquia(LlantaCrearViewModel model, int idEmpresa)
        {
            bool regionValida = await context.Regions.AnyAsync(r => r.IdRegion == model.IdRegion && r.IdEmpresa == idEmpresa);
            if (!regionValida)
                throw new Exception("La región seleccionada no pertenece a su empresa.");

            bool plantaValida = await context.Planta.AnyAsync(p => p.IdPlanta == model.IdPlanta && p.IdRegion == model.IdRegion && p.IdEmpresa == idEmpresa);
            if (!plantaValida)
                throw new Exception("La planta seleccionada no pertenece a la región y empresa indicadas.");

            bool zonaValida = await context.Zonas.AnyAsync(z => z.IdZona == model.IdZona && z.IdRegion == model.IdRegion && z.IdPlanta == model.IdPlanta && z.IdEmpresa == idEmpresa);
            if (!zonaValida)
                throw new Exception("La zona seleccionada no pertenece a la planta, región y empresa indicadas.");

            bool depositoValido = await context.Depositos.AnyAsync(d => d.IdDeposito == model.IdDeposito && d.IdRegion == model.IdRegion && d.IdPlanta == model.IdPlanta && d.IdZona == model.IdZona && d.IdEmpresa == idEmpresa);
            if (!depositoValido)
                throw new Exception("El depósito seleccionado no pertenece a la zona, planta, región y empresa indicadas.");
        }
    }
}
