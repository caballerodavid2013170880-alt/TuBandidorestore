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
                        Marca = llanta.IdModeloLlantaNavigation.IdMarcaLlantaNavigation.Nombre,
                        Modelo = llanta.IdModeloLlantaNavigation.Nombre,
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

            if (vRet.IdEstadoLlanta <= 0)
            {
                vRet.IdEstadoLlanta = 1;
            }

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

            vRet.Marcas = await context.LlantaMarcas
                .AsNoTracking()
                .Where(m => m.Activo == true)
                .OrderBy(m => m.Nombre)
                .Select(m => new LlantaCrearViewModel.CatalogItemViewModel
                {
                    Id = (int)m.IdMarcaLlanta,
                    Nombre = m.Nombre
                })
                .ToListAsync();

            if (vRet.IdMarcaLlanta > 0)
            {
                vRet.Modelos = await GetModelosPorMarca(vRet.IdMarcaLlanta);
            }

            vRet.EstadosLlanta = await context.LlantaEstados
                .AsNoTracking()
                .Where(e => e.EsActivo == true)
                .OrderBy(e => e.IdEstadoLlanta)
                .Select(e => new LlantaCrearViewModel.CatalogItemViewModel
                {
                    Id = e.IdEstadoLlanta,
                    Nombre = e.Nombre
                })
                .ToListAsync();

            if (vRet.IdModeloLlanta > 0)
            {
                vRet.ModeloDetalle = await GetDetalleModelo(vRet.IdModeloLlanta);
            }

            return vRet;
        }

        public async Task<List<LlantaCrearViewModel.CatalogItemViewModel>> GetModelosPorMarca(int idMarcaLlanta)
        {
            return await context.LlantaModelos
                .AsNoTracking()
                .Where(m => m.Activo == true && m.IdMarcaLlanta == (uint)idMarcaLlanta)
                .OrderBy(m => m.Nombre)
                .ThenBy(m => m.Medida)
                .Select(m => new LlantaCrearViewModel.CatalogItemViewModel
                {
                    Id = (int)m.IdModeloLlanta,
                    Nombre = string.IsNullOrWhiteSpace(m.Medida) ? m.Nombre : $"{m.Nombre} - {m.Medida}"
                })
                .ToListAsync();
        }

        public async Task<LlantaCrearViewModel.ModeloDetalleViewModel?> GetDetalleModelo(int idModeloLlanta)
        {
            return await context.LlantaModelos
                .AsNoTracking()
                .Where(m => m.Activo == true && m.IdModeloLlanta == (uint)idModeloLlanta)
                .Select(m => new LlantaCrearViewModel.ModeloDetalleViewModel
                {
                    IdModeloLlanta = (int)m.IdModeloLlanta,
                    Marca = m.IdMarcaLlantaNavigation.Nombre,
                    Modelo = m.Nombre,
                    Medida = m.Medida,
                    PresionMinimaPsi = m.PresionMinimaPsi,
                    PresionMaximaPsi = m.PresionMaximaPsi,
                    ProfundidadOriginalMm = m.ProfundidadOriginalMm,
                    VidaUtilEstimadaKm = m.VidaUtilEstimadaKm.HasValue ? (int)m.VidaUtilEstimadaKm.Value : null
                })
                .FirstOrDefaultAsync();
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
            await ValidarCatalogos(model);

            var llanta = new Llantum
            {
                CodigoLlanta = model.CodigoLlanta.Trim(),
                NumeroSerieDot = model.NumeroSerieDot.Trim(),
                IdModeloLlanta = (uint)model.IdModeloLlanta,
                IdEstadoLlanta = (ushort)model.IdEstadoLlanta,
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

            if (model.IdMarcaLlanta <= 0)
                throw new Exception("La marca es obligatoria.");

            if (model.IdModeloLlanta <= 0)
                throw new Exception("El modelo es obligatorio.");

            if (model.IdEstadoLlanta <= 0)
                throw new Exception("El estado de la llanta es obligatorio.");

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

            if (model.FechaFabricacion.HasValue && model.FechaAdquisicion.HasValue && model.FechaFabricacion.Value.Date > model.FechaAdquisicion.Value.Date)
                throw new Exception("La fecha de fabricación no puede ser posterior a la fecha de adquisición.");
        }

        private async Task ValidarCatalogos(LlantaCrearViewModel model)
        {
            bool marcaValida = await context.LlantaMarcas
                .AnyAsync(m => m.IdMarcaLlanta == (uint)model.IdMarcaLlanta && m.Activo == true);
            if (!marcaValida)
                throw new Exception("La marca seleccionada no existe o no está activa.");

            bool modeloValido = await context.LlantaModelos
                .AnyAsync(m => m.IdModeloLlanta == (uint)model.IdModeloLlanta
                            && m.IdMarcaLlanta == (uint)model.IdMarcaLlanta
                            && m.Activo == true);
            if (!modeloValido)
                throw new Exception("El modelo seleccionado no pertenece a la marca o no está activo.");

            bool estadoValido = await context.LlantaEstados
                .AnyAsync(e => e.IdEstadoLlanta == (ushort)model.IdEstadoLlanta && e.EsActivo == true);
            if (!estadoValido)
                throw new Exception("El estado seleccionado no existe o no está activo.");

            if (model.IdEstadoLlanta == 1 && model.IdDeposito <= 0)
                throw new Exception("Debes seleccionar un depósito para una llanta en depósito.");
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
