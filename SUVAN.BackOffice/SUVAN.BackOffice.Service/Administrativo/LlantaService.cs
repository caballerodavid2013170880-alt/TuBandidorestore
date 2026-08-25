using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class LlantaService : ILlantaService
    {
        private const ushort TipoAsignacionInicial = 1;
        private const ushort TipoAsignacionReemplazo = 2;
        private const ushort TipoAsignacionRotacion = 3;

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

        public async Task<LlantaCrearViewModel?> GetEditarViewModel(ulong idLlanta, int idEmpresa, string nombreEmpresa)
        {
            var llanta = await context.Llanta
                .AsNoTracking()
                .Include(x => x.IdModeloLlantaNavigation)
                .FirstOrDefaultAsync(x => x.IdLlanta == idLlanta
                                       && !x.Eliminado
                                       && x.IdEmpresa == (uint)idEmpresa);

            if (llanta == null)
            {
                return null;
            }

            var model = new LlantaCrearViewModel
            {
                IdLlanta = llanta.IdLlanta,
                CodigoLlanta = llanta.CodigoLlanta,
                NumeroSerieDot = llanta.NumeroSerieDot,
                IdMarcaLlanta = (int)llanta.IdModeloLlantaNavigation.IdMarcaLlanta,
                IdModeloLlanta = (int)llanta.IdModeloLlanta,
                IdEstadoLlanta = llanta.IdEstadoLlanta,
                IdRegion = (int)(llanta.IdRegion ?? 0),
                IdPlanta = (int)(llanta.IdPlanta ?? 0),
                IdZona = (int)(llanta.IdZona ?? 0),
                IdDeposito = (int)(llanta.IdDeposito ?? 0),
                FechaFabricacion = llanta.FechaFabricacion.HasValue ? llanta.FechaFabricacion.Value.ToDateTime(TimeOnly.MinValue) : null,
                CostoAdquisicion = llanta.CostoAdquisicion,
                FechaAdquisicion = llanta.FechaAdquisicion.HasValue ? llanta.FechaAdquisicion.Value.ToDateTime(TimeOnly.MinValue) : null,
                Observaciones = llanta.Observaciones
            };

            return await GetCrearViewModel(idEmpresa, nombreEmpresa, model);
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

            await ValidarDuplicados(model);
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

        public async Task<bool> ActualizarLlanta(LlantaCrearViewModel model, int idEmpresa, int idUsuario)
        {
            if (model.IdLlanta == 0)
                throw new Exception("No se encontró la llanta a editar.");

            ValidarDatosCaptura(model);
            await ValidarDuplicados(model);
            await ValidarJerarquia(model, idEmpresa);
            await ValidarCatalogos(model);

            var llanta = await context.Llanta
                .FirstOrDefaultAsync(x => x.IdLlanta == model.IdLlanta
                                       && !x.Eliminado
                                       && x.IdEmpresa == (uint)idEmpresa);

            if (llanta == null)
                throw new Exception("No se encontró la llanta o no pertenece a su empresa.");

            llanta.CodigoLlanta = model.CodigoLlanta.Trim();
            llanta.NumeroSerieDot = model.NumeroSerieDot.Trim();
            llanta.IdModeloLlanta = (uint)model.IdModeloLlanta;
            llanta.IdEstadoLlanta = (ushort)model.IdEstadoLlanta;
            llanta.IdRegion = (uint)model.IdRegion;
            llanta.IdPlanta = (uint)model.IdPlanta;
            llanta.IdZona = (uint)model.IdZona;
            llanta.IdDeposito = (uint)model.IdDeposito;
            llanta.FechaFabricacion = model.FechaFabricacion.HasValue ? DateOnly.FromDateTime(model.FechaFabricacion.Value) : null;
            llanta.CostoAdquisicion = model.CostoAdquisicion;
            llanta.FechaAdquisicion = DateOnly.FromDateTime(model.FechaAdquisicion!.Value);
            llanta.Observaciones = string.IsNullOrWhiteSpace(model.Observaciones) ? null : model.Observaciones.Trim();
            llanta.FechaModificacion = DateTime.Now;
            llanta.ModificadoPor = (uint)idUsuario;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarLlanta(ulong idLlanta, int idEmpresa, int idUsuario)
        {
            var llanta = await context.Llanta
                .FirstOrDefaultAsync(x => x.IdLlanta == idLlanta
                                       && !x.Eliminado
                                       && x.IdEmpresa == (uint)idEmpresa);

            if (llanta == null)
                throw new Exception("No se encontró la llanta o no pertenece a su empresa.");

            llanta.Eliminado = true;
            llanta.FechaEliminacion = DateTime.Now;
            llanta.EliminadoPor = (uint)idUsuario;
            llanta.MotivoEliminacion = "Eliminación lógica desde BackOffice.";
            llanta.FechaModificacion = DateTime.Now;
            llanta.ModificadoPor = (uint)idUsuario;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<LlantaConfiguracionVehiculoViewModel> GetConfiguracionVehiculoLlantas(int idVehiculo, int idEmpresa)
        {
            var vehiculo = await context.Vehiculos
                .AsNoTracking()
                .Where(x => x.IdVehiculo == idVehiculo && x.EmpresaIdempresa == idEmpresa)
                .Select(x => new
                {
                    x.IdVehiculo,
                    x.Numeroeconomico,
                    x.Placas,
                    x.Marca,
                    x.Modelo,
                    KilometrajeActual = x.VehiculoDetalles
                        .OrderByDescending(d => d.IdVehiculoDetalle)
                        .Select(d => d.KilometrajeAcumulado)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (vehiculo == null)
                throw new Exception("No se encontró el vehículo o no pertenece a su empresa.");

            var ejes = await context.VehiculoEjes
                .AsNoTracking()
                .Where(x => x.IdVehiculo == idVehiculo && x.Activo == true)
                .OrderBy(x => x.NumeroEje)
                .Select(x => new
                {
                    x.IdVehiculoEje,
                    x.NumeroEje,
                    x.IdTipoEje,
                    NombreTipoEje = x.IdTipoEjeNavigation.Nombre,
                    DescripcionTipoEje = x.IdTipoEjeNavigation.Descripcion,
                    x.IdTipoEjeNavigation.NumeroPosiciones
                })
                .ToListAsync();

            var asignacionesActivas = await context.LlantaAsignacions
                .AsNoTracking()
                .Where(x => x.IdVehiculo == idVehiculo && x.Activa == true)
                .Select(x => new
                {
                    x.IdLlantaAsignacion,
                    x.IdVehiculoEje,
                    x.NumeroPosicion,
                    x.IdLlanta,
                    x.IdLlantaNavigation.CodigoLlanta,
                    x.IdLlantaNavigation.NumeroSerieDot,
                    Marca = x.IdLlantaNavigation.IdModeloLlantaNavigation.IdMarcaLlantaNavigation.Nombre,
                    Modelo = x.IdLlantaNavigation.IdModeloLlantaNavigation.Nombre,
                    Medida = x.IdLlantaNavigation.IdModeloLlantaNavigation.Medida,
                    x.IdLlantaNavigation.IdEstadoLlanta,
                    EstadoLlanta = x.IdLlantaNavigation.IdEstadoLlantaNavigation.Nombre,
                    x.FechaAsignacion,
                    x.KmVehiculoAsignacion,
                    x.IdLlantaNavigation.IdModeloLlantaNavigation.PresionMinimaPsi,
                    x.IdLlantaNavigation.IdModeloLlantaNavigation.PresionMaximaPsi,
                    x.IdLlantaNavigation.IdModeloLlantaNavigation.ProfundidadOriginalMm,
                    x.IdLlantaNavigation.IdModeloLlantaNavigation.ProfundidadAlertaMm,
                    x.IdLlantaNavigation.IdModeloLlantaNavigation.ProfundidadMinimaMm,
                    x.IdLlantaNavigation.IdModeloLlantaNavigation.VidaUtilEstimadaKm
                })
                .ToListAsync();

            var asignacionesPorPosicion = asignacionesActivas
                .GroupBy(x => new { x.IdVehiculoEje, x.NumeroPosicion })
                .ToDictionary(x => x.Key, x => x.OrderByDescending(a => a.FechaAsignacion).First());

            return new LlantaConfiguracionVehiculoViewModel
            {
                IdVehiculo = vehiculo.IdVehiculo,
                NumeroEconomico = vehiculo.Numeroeconomico,
                Placas = vehiculo.Placas,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                KilometrajeActual = vehiculo.KilometrajeActual,
                Ejes = ejes.Select(eje => new LlantaConfiguracionEjeViewModel
                {
                    IdVehiculoEje = eje.IdVehiculoEje,
                    NumeroEje = eje.NumeroEje,
                    IdTipoEje = eje.IdTipoEje,
                    NombreTipoEje = eje.NombreTipoEje,
                    DescripcionTipoEje = eje.DescripcionTipoEje,
                    NumeroPosiciones = eje.NumeroPosiciones,
                    Posiciones = Enumerable.Range(1, eje.NumeroPosiciones)
                        .Select(numeroPosicion =>
                        {
                            var posicion = (ushort)numeroPosicion;
                            asignacionesPorPosicion.TryGetValue(
                                new { eje.IdVehiculoEje, NumeroPosicion = posicion },
                                out var asignacion);

                            return new LlantaConfiguracionPosicionViewModel
                            {
                                NumeroPosicion = posicion,
                                Ocupada = asignacion != null,
                                Asignacion = asignacion == null
                                    ? null
                                    : new LlantaConfiguracionAsignacionViewModel
                                    {
                                        IdLlantaAsignacion = asignacion.IdLlantaAsignacion,
                                        IdLlanta = asignacion.IdLlanta,
                                        CodigoLlanta = asignacion.CodigoLlanta,
                                        NumeroSerieDot = asignacion.NumeroSerieDot,
                                        Marca = asignacion.Marca,
                                        Modelo = asignacion.Modelo,
                                        Medida = asignacion.Medida,
                                        IdEstadoLlanta = asignacion.IdEstadoLlanta,
                                        EstadoLlanta = asignacion.EstadoLlanta,
                                        FechaAsignacion = asignacion.FechaAsignacion,
                                        KmVehiculoAsignacion = asignacion.KmVehiculoAsignacion,
                                        PresionMinimaPsi = asignacion.PresionMinimaPsi,
                                        PresionMaximaPsi = asignacion.PresionMaximaPsi,
                                        ProfundidadOriginalMm = asignacion.ProfundidadOriginalMm,
                                        ProfundidadAlertaMm = asignacion.ProfundidadAlertaMm,
                                        ProfundidadMinimaMm = asignacion.ProfundidadMinimaMm,
                                        VidaUtilEstimadaKm = asignacion.VidaUtilEstimadaKm
                                    }
                            };
                        })
                        .ToList()
                })
                .ToList()
            };
        }

        public async Task<List<LlantaCrearViewModel.CatalogItemViewModel>> GetVehiculosParaAsignacion(int idEmpresa)
        {
            return await context.Vehiculos
                .AsNoTracking()
                //.Where(x => x.EmpresaIdempresa == idEmpresa && x.Activo == 1)
                .OrderBy(x => x.Numeroeconomico)
                .ThenBy(x => x.Placas)
                .Select(x => new LlantaCrearViewModel.CatalogItemViewModel
                {
                    Id = x.IdVehiculo,
                    Nombre = string.IsNullOrWhiteSpace(x.Numeroeconomico)
                        ? x.Placas
                        : string.IsNullOrWhiteSpace(x.Placas)
                            ? x.Numeroeconomico
                            : $"{x.Numeroeconomico} - {x.Placas}"
                })
                .ToListAsync();
        }

        public async Task<List<LlantaCrearViewModel.CatalogItemViewModel>> GetLlantasDisponiblesParaInstalacion(int idEmpresa)
        {
            var estadosInstalables = await context.LlantaEstados
                .AsNoTracking()
                .Where(x => x.EsActivo == true)
                .Select(x => new { x.IdEstadoLlanta, x.Nombre })
                .ToListAsync();

            var idsEstadosInstalables = estadosInstalables
                .Where(x =>
                {
                    var nombre = NormalizarTexto(x.Nombre);
                    return !nombre.Contains("instalada")
                        && !nombre.Contains("inspeccion")
                        && !nombre.Contains("reparacion")
                        && !nombre.Contains("renovado")
                        && !nombre.Contains("fuera")
                        && !nombre.Contains("baja");
                })
                .Select(x => x.IdEstadoLlanta)
                .ToList();

            return await context.Llanta
                .AsNoTracking()
                .Where(x => !x.Eliminado
                         && x.IdEmpresa == (uint)idEmpresa
                         && idsEstadosInstalables.Contains(x.IdEstadoLlanta)
                         && !context.LlantaAsignacions.Any(a => a.IdLlanta == x.IdLlanta && a.Activa == true))
                .OrderBy(x => x.CodigoLlanta)
                .Select(x => new LlantaCrearViewModel.CatalogItemViewModel
                {
                    Id = (int)x.IdLlanta,
                    Nombre = x.CodigoLlanta + " - " + x.NumeroSerieDot + " / " + x.IdModeloLlantaNavigation.IdMarcaLlantaNavigation.Nombre + " " + x.IdModeloLlantaNavigation.Nombre
                })
                .ToListAsync();
        }

        public async Task<bool> InstalarLlanta(LlantaInstalacionViewModel model, int idEmpresa, int idUsuario)
        {
            if (model.IdVehiculo <= 0)
                throw new Exception("El vehículo es obligatorio.");

            if (model.IdVehiculoEje <= 0)
                throw new Exception("El eje es obligatorio.");

            if (model.NumeroPosicion <= 0)
                throw new Exception("La posición es obligatoria.");

            if (model.IdLlanta == 0)
                throw new Exception("La llanta es obligatoria.");

            if (model.FechaAsignacion == default)
                throw new Exception("La fecha de instalación es obligatoria.");

            if (model.ObservacionesAsignacion?.Length > 500)
                throw new Exception("Las observaciones no deben exceder 500 caracteres.");

            var hoy = DateTime.Today;
            if (model.FechaAsignacion.Date > hoy)
                throw new Exception("La fecha de instalación no puede ser posterior a la fecha actual.");

            await using var transaction = await context.Database.BeginTransactionAsync();

            var vehiculoExiste = await context.Vehiculos
                .AnyAsync(x => x.IdVehiculo == model.IdVehiculo && x.EmpresaIdempresa == idEmpresa);

            if (!vehiculoExiste)
                throw new Exception("No se encontró el vehículo o no pertenece a su empresa.");

            var eje = await context.VehiculoEjes
                .Include(x => x.IdTipoEjeNavigation)
                .FirstOrDefaultAsync(x => x.IdVehiculoEje == model.IdVehiculoEje
                                       && x.IdVehiculo == model.IdVehiculo
                                       && x.Activo == true);

            if (eje == null)
                throw new Exception("No se encontró el eje o no pertenece al vehículo.");

            if (model.NumeroPosicion > eje.IdTipoEjeNavigation.NumeroPosiciones)
                throw new Exception("La posición seleccionada no pertenece al eje.");

            var posicionOcupada = await context.LlantaAsignacions
                .AnyAsync(x => x.IdVehiculoEje == model.IdVehiculoEje
                            && x.NumeroPosicion == model.NumeroPosicion
                            && x.Activa == true);

            if (posicionOcupada)
                throw new Exception("La posición seleccionada ya tiene una llanta instalada.");

            var llanta = await context.Llanta
                .FirstOrDefaultAsync(x => x.IdLlanta == model.IdLlanta
                                       && x.IdEmpresa == (uint)idEmpresa
                                       && !x.Eliminado);

            if (llanta == null)
                throw new Exception("No se encontró la llanta o no pertenece a su empresa.");

            var llantaConAsignacionActiva = await context.LlantaAsignacions
                .AnyAsync(x => x.IdLlanta == model.IdLlanta && x.Activa == true);

            if (llantaConAsignacionActiva)
                throw new Exception("La llanta seleccionada ya tiene una asignación activa.");

            var estadoActualLlanta = await context.LlantaEstados
                .AsNoTracking()
                .Where(x => x.IdEstadoLlanta == llanta.IdEstadoLlanta)
                .Select(x => x.Nombre)
                .FirstOrDefaultAsync();

            if (!EstadoPermiteInstalacion(estadoActualLlanta))
                throw new Exception("La llanta seleccionada no está disponible para instalación.");

            var existeTipoAsignacionInicial = await context.LlantaTipoAsignacions
                .AsNoTracking()
                .AnyAsync(x => x.IdTipoAsignacion == TipoAsignacionInicial && x.EsActivo == true);

            if (!existeTipoAsignacionInicial)
                throw new Exception("No se encontró el tipo de asignación inicial activo con id 1.");

            var idEstadoInstalada = await GetIdEstadoInstalada();

            var kilometrajesVehiculo = await context.VehiculoDetalles
                .AsNoTracking()
                .Where(x => x.IdVehiculo == model.IdVehiculo && x.KilometrajeAcumulado.HasValue)
                .Select(x => x.KilometrajeAcumulado!.Value)
                .ToListAsync();

            var kilometrajesAsignacion = await context.LlantaAsignacions
                .AsNoTracking()
                .Where(x => x.IdVehiculo == model.IdVehiculo)
                .Select(x => new { x.KmVehiculoAsignacion, x.KmVehiculoRetiro })
                .ToListAsync();

            var ultimoKmVehiculo = kilometrajesVehiculo.Any()
                ? kilometrajesVehiculo.Max()
                : 0;
            var ultimoKmAsignacion = kilometrajesAsignacion
                .SelectMany(x => new[] { (uint?)x.KmVehiculoAsignacion, x.KmVehiculoRetiro })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .DefaultIfEmpty((uint)0)
                .Max();
            var ultimoKmValido = Math.Max((decimal)ultimoKmVehiculo, (decimal)ultimoKmAsignacion);

            if (model.KmVehiculoAsignacion < ultimoKmValido)
                throw new Exception($"El kilometraje no puede ser menor al último kilometraje registrado ({ultimoKmValido:0}).");

            var asignacion = new LlantaAsignacion
            {
                IdLlanta = model.IdLlanta,
                IdVehiculo = model.IdVehiculo,
                IdVehiculoEje = model.IdVehiculoEje,
                NumeroPosicion = model.NumeroPosicion,
                IdTipoAsignacion = TipoAsignacionInicial,
                FechaAsignacion = model.FechaAsignacion,
                KmVehiculoAsignacion = model.KmVehiculoAsignacion,
                FechaRetiro = null,
                KmVehiculoRetiro = null,
                IdMotivoRetiro = null,
                ObservacionesAsignacion = string.IsNullOrWhiteSpace(model.ObservacionesAsignacion) ? null : model.ObservacionesAsignacion.Trim(),
                ObservacionesRetiro = null,
                Activa = true,
                FechaCreacion = DateTime.Now,
                CreadoPor = (uint)idUsuario,
                FechaModificacion = null,
                ModificadoPor = null,
                FechaEliminacion = null,
                EliminadoPor = null
            };

            llanta.IdEstadoLlanta = idEstadoInstalada;
            llanta.FechaModificacion = DateTime.Now;
            llanta.ModificadoPor = (uint)idUsuario;

            context.LlantaAsignacions.Add(asignacion);
            await context.SaveChangesAsync();

            RegistrarMovimientoBitacora(new LlantaAsignacionBitacoraMovimiento
            {
                IdLlanta = asignacion.IdLlanta,
                TipoMovimiento = LlantaMovimientoBitacoraTipo.Asignacion,
                IdLlantaAsignacionDestino = asignacion.IdLlantaAsignacion,
                IdVehiculoDestino = (ulong)asignacion.IdVehiculo,
                IdVehiculoEjeDestino = (ulong)asignacion.IdVehiculoEje,
                PosicionDestino = asignacion.NumeroPosicion,
                Kilometraje = asignacion.KmVehiculoAsignacion,
                CreadoPor = (uint)idUsuario
            });

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        public async Task<List<LlantaCrearViewModel.CatalogItemViewModel>> GetMotivosRetiroActivos()
        {
            return await context.LlantaMotivoRetiros
                .AsNoTracking()
                .Where(x => x.EsActivo == true)
                .OrderBy(x => x.Nombre)
                .Select(x => new LlantaCrearViewModel.CatalogItemViewModel
                {
                    Id = x.IdMotivoRetiro,
                    Nombre = x.Nombre
                })
                .ToListAsync();
        }

        public async Task<List<LlantaCrearViewModel.CatalogItemViewModel>> GetEstadosDestinoRetiro()
        {
            var estados = await context.LlantaEstados
                .AsNoTracking()
                .Where(x => x.EsActivo == true)
                .OrderBy(x => x.IdEstadoLlanta)
                .Select(x => new { x.IdEstadoLlanta, x.Nombre })
                .ToListAsync();

            return estados
                .Where(x => !NormalizarTexto(x.Nombre).Contains("instalada"))
                .Select(x => new LlantaCrearViewModel.CatalogItemViewModel
                {
                    Id = x.IdEstadoLlanta,
                    Nombre = x.Nombre
                })
                .ToList();
        }

        public async Task<bool> RetirarLlanta(LlantaRetiroViewModel model, int idEmpresa, int idUsuario, bool administrarTransaccion = true)
        {
            if (model.IdLlantaAsignacion == 0)
                throw new Exception("La asignación es obligatoria.");

            if (model.FechaRetiro == default)
                throw new Exception("La fecha de retiro es obligatoria.");

            if (model.IdMotivoRetiro <= 0)
                throw new Exception("El motivo de retiro es obligatorio.");

            if (model.IdEstadoDestino <= 0)
                throw new Exception("El estado destino es obligatorio.");

            if (model.ObservacionesRetiro?.Length > 500)
                throw new Exception("Las observaciones no deben exceder 500 caracteres.");

            if (model.FechaRetiro.Date > DateTime.Today)
                throw new Exception("La fecha de retiro no puede ser posterior a la fecha actual.");

            await using var transaction = administrarTransaccion
                ? await context.Database.BeginTransactionAsync()
                : null;

            var asignacion = await context.LlantaAsignacions
                .Include(x => x.IdLlantaNavigation)
                .Include(x => x.IdVehiculoNavigation)
                .FirstOrDefaultAsync(x => x.IdLlantaAsignacion == model.IdLlantaAsignacion
                                       && x.Activa == true);

            if (asignacion == null)
                throw new Exception("No se encontró una asignación activa para retirar.");

            if (asignacion.IdVehiculoNavigation.EmpresaIdempresa != idEmpresa)
                throw new Exception("La asignación no pertenece a su empresa.");

            if (model.FechaRetiro < asignacion.FechaAsignacion)
                throw new Exception("La fecha de retiro no puede ser anterior a la fecha de instalación.");

            var motivoValido = await context.LlantaMotivoRetiros
                .AsNoTracking()
                .AnyAsync(x => x.IdMotivoRetiro == model.IdMotivoRetiro && x.EsActivo == true);

            if (!motivoValido)
                throw new Exception("El motivo de retiro no está activo o no existe.");

            var estadoDestino = await context.LlantaEstados
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdEstadoLlanta == model.IdEstadoDestino && x.EsActivo == true);

            if (estadoDestino == null)
                throw new Exception("El estado destino no está activo o no existe.");

            if (NormalizarTexto(estadoDestino.Nombre).Contains("instalada"))
                throw new Exception("El estado destino no puede ser Instalada para un retiro.");

            var ultimoKmValido = await GetUltimoKilometrajeValido(asignacion.IdVehiculo);
            if (model.KmVehiculoRetiro < ultimoKmValido)
                throw new Exception($"El kilometraje no puede ser menor al último kilometraje registrado ({ultimoKmValido:0}).");

            if (model.KmVehiculoRetiro < asignacion.KmVehiculoAsignacion)
                throw new Exception($"El kilometraje de retiro no puede ser menor al kilometraje de instalación ({asignacion.KmVehiculoAsignacion}).");

            asignacion.FechaRetiro = model.FechaRetiro;
            asignacion.KmVehiculoRetiro = model.KmVehiculoRetiro;
            asignacion.IdMotivoRetiro = model.IdMotivoRetiro;
            asignacion.ObservacionesRetiro = string.IsNullOrWhiteSpace(model.ObservacionesRetiro) ? null : model.ObservacionesRetiro.Trim();
            asignacion.Activa = false;
            asignacion.FechaModificacion = DateTime.Now;
            asignacion.ModificadoPor = (uint)idUsuario;

            asignacion.IdLlantaNavigation.IdEstadoLlanta = model.IdEstadoDestino;
            asignacion.IdLlantaNavigation.FechaModificacion = DateTime.Now;
            asignacion.IdLlantaNavigation.ModificadoPor = (uint)idUsuario;

            RegistrarMovimientoBitacora(new LlantaAsignacionBitacoraMovimiento
            {
                IdLlanta = asignacion.IdLlanta,
                TipoMovimiento = LlantaMovimientoBitacoraTipo.Retiro,
                IdLlantaAsignacionOrigen = asignacion.IdLlantaAsignacion,
                IdVehiculoOrigen = (ulong)asignacion.IdVehiculo,
                IdVehiculoEjeOrigen = (ulong)asignacion.IdVehiculoEje,
                PosicionOrigen = asignacion.NumeroPosicion,
                Kilometraje = model.KmVehiculoRetiro,
                CreadoPor = (uint)idUsuario
            });

            await context.SaveChangesAsync();
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            return true;
        }

        public async Task<bool> ReemplazarLlanta(LlantaReemplazoViewModel model, int idEmpresa, int idUsuario)
        {
            if (model.IdLlantaAsignacion == 0)
                throw new Exception("La asignación saliente es obligatoria.");

            if (model.IdLlantaEntrante == 0)
                throw new Exception("La llanta entrante es obligatoria.");

            if (model.FechaMovimiento == default)
                throw new Exception("La fecha del reemplazo es obligatoria.");

            if (model.IdMotivoRetiro <= 0)
                throw new Exception("El motivo de retiro es obligatorio.");

            if (model.IdEstadoDestinoSaliente <= 0)
                throw new Exception("El estado destino de la llanta saliente es obligatorio.");

            if (model.ObservacionesRetiro?.Length > 500 || model.ObservacionesAsignacion?.Length > 500)
                throw new Exception("Las observaciones no deben exceder 500 caracteres.");

            if (model.FechaMovimiento.Date > DateTime.Today)
                throw new Exception("La fecha del reemplazo no puede ser posterior a la fecha actual.");

            await using var transaction = await context.Database.BeginTransactionAsync();

            var asignacionSaliente = await context.LlantaAsignacions
                .Include(x => x.IdLlantaNavigation)
                .Include(x => x.IdVehiculoNavigation)
                .FirstOrDefaultAsync(x => x.IdLlantaAsignacion == model.IdLlantaAsignacion
                                       && x.Activa == true);

            if (asignacionSaliente == null)
                throw new Exception("No se encontró una asignación activa para reemplazar.");

            if (asignacionSaliente.IdVehiculoNavigation.EmpresaIdempresa != idEmpresa)
                throw new Exception("La asignación no pertenece a su empresa.");

            if (model.FechaMovimiento < asignacionSaliente.FechaAsignacion)
                throw new Exception("La fecha del reemplazo no puede ser anterior a la fecha de instalación.");

            if (asignacionSaliente.IdLlanta == model.IdLlantaEntrante)
                throw new Exception("La llanta entrante debe ser distinta a la llanta saliente.");

            var motivoValido = await context.LlantaMotivoRetiros
                .AsNoTracking()
                .AnyAsync(x => x.IdMotivoRetiro == model.IdMotivoRetiro && x.EsActivo == true);

            if (!motivoValido)
                throw new Exception("El motivo de retiro no está activo o no existe.");

            var estadoSaliente = await context.LlantaEstados
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdEstadoLlanta == model.IdEstadoDestinoSaliente && x.EsActivo == true);

            if (estadoSaliente == null)
                throw new Exception("El estado destino de la llanta saliente no está activo o no existe.");

            if (NormalizarTexto(estadoSaliente.Nombre).Contains("instalada"))
                throw new Exception("El estado destino de la llanta saliente no puede ser Instalada.");

            var existeTipoReemplazo = await context.LlantaTipoAsignacions
                .AsNoTracking()
                .AnyAsync(x => x.IdTipoAsignacion == TipoAsignacionReemplazo && x.EsActivo == true);

            if (!existeTipoReemplazo)
                throw new Exception("No se encontró el tipo de asignación Reemplazo activo con id 2.");

            var idEstadoInstalada = await GetIdEstadoInstalada();

            var llantaEntrante = await context.Llanta
                .FirstOrDefaultAsync(x => x.IdLlanta == model.IdLlantaEntrante
                                       && x.IdEmpresa == (uint)idEmpresa
                                       && !x.Eliminado);

            if (llantaEntrante == null)
                throw new Exception("No se encontró la llanta entrante o no pertenece a su empresa.");

            var llantaEntranteConAsignacionActiva = await context.LlantaAsignacions
                .AnyAsync(x => x.IdLlanta == model.IdLlantaEntrante && x.Activa == true);

            if (llantaEntranteConAsignacionActiva)
                throw new Exception("La llanta entrante ya tiene una asignación activa.");

            var estadoActualEntrante = await context.LlantaEstados
                .AsNoTracking()
                .Where(x => x.IdEstadoLlanta == llantaEntrante.IdEstadoLlanta)
                .Select(x => x.Nombre)
                .FirstOrDefaultAsync();

            if (!EstadoPermiteInstalacion(estadoActualEntrante))
                throw new Exception("La llanta entrante no está disponible para instalación.");

            var ultimoKmValido = await GetUltimoKilometrajeValido(asignacionSaliente.IdVehiculo);
            if (model.KmVehiculo < ultimoKmValido)
                throw new Exception($"El kilometraje no puede ser menor al último kilometraje registrado ({ultimoKmValido:0}).");

            if (model.KmVehiculo < asignacionSaliente.KmVehiculoAsignacion)
                throw new Exception($"El kilometraje del reemplazo no puede ser menor al kilometraje de instalación ({asignacionSaliente.KmVehiculoAsignacion}).");

            asignacionSaliente.FechaRetiro = model.FechaMovimiento;
            asignacionSaliente.KmVehiculoRetiro = model.KmVehiculo;
            asignacionSaliente.IdMotivoRetiro = model.IdMotivoRetiro;
            asignacionSaliente.ObservacionesRetiro = string.IsNullOrWhiteSpace(model.ObservacionesRetiro) ? null : model.ObservacionesRetiro.Trim();
            asignacionSaliente.Activa = false;
            asignacionSaliente.FechaModificacion = DateTime.Now;
            asignacionSaliente.ModificadoPor = (uint)idUsuario;

            asignacionSaliente.IdLlantaNavigation.IdEstadoLlanta = model.IdEstadoDestinoSaliente;
            asignacionSaliente.IdLlantaNavigation.FechaModificacion = DateTime.Now;
            asignacionSaliente.IdLlantaNavigation.ModificadoPor = (uint)idUsuario;

            var asignacionEntrante = new LlantaAsignacion
            {
                IdLlanta = model.IdLlantaEntrante,
                IdVehiculo = asignacionSaliente.IdVehiculo,
                IdVehiculoEje = asignacionSaliente.IdVehiculoEje,
                NumeroPosicion = asignacionSaliente.NumeroPosicion,
                IdTipoAsignacion = TipoAsignacionReemplazo,
                FechaAsignacion = model.FechaMovimiento,
                KmVehiculoAsignacion = model.KmVehiculo,
                FechaRetiro = null,
                KmVehiculoRetiro = null,
                IdMotivoRetiro = null,
                ObservacionesAsignacion = string.IsNullOrWhiteSpace(model.ObservacionesAsignacion) ? null : model.ObservacionesAsignacion.Trim(),
                ObservacionesRetiro = null,
                Activa = true,
                FechaCreacion = DateTime.Now,
                CreadoPor = (uint)idUsuario,
                FechaModificacion = null,
                ModificadoPor = null,
                FechaEliminacion = null,
                EliminadoPor = null
            };

            llantaEntrante.IdEstadoLlanta = idEstadoInstalada;
            llantaEntrante.FechaModificacion = DateTime.Now;
            llantaEntrante.ModificadoPor = (uint)idUsuario;

            await context.SaveChangesAsync();
            context.LlantaAsignacions.Add(asignacionEntrante);
            await context.SaveChangesAsync();

            var idOperacion = Guid.NewGuid();

            RegistrarMovimientoBitacora(new LlantaAsignacionBitacoraMovimiento
            {
                IdOperacion = idOperacion,
                IdLlanta = asignacionSaliente.IdLlanta,
                TipoMovimiento = LlantaMovimientoBitacoraTipo.Reemplazo,
                IdLlantaAsignacionOrigen = asignacionSaliente.IdLlantaAsignacion,
                IdVehiculoOrigen = (ulong)asignacionSaliente.IdVehiculo,
                IdVehiculoEjeOrigen = (ulong)asignacionSaliente.IdVehiculoEje,
                PosicionOrigen = asignacionSaliente.NumeroPosicion,
                Kilometraje = model.KmVehiculo,
                CreadoPor = (uint)idUsuario
            });

            RegistrarMovimientoBitacora(new LlantaAsignacionBitacoraMovimiento
            {
                IdOperacion = idOperacion,
                IdLlanta = asignacionEntrante.IdLlanta,
                TipoMovimiento = LlantaMovimientoBitacoraTipo.Reemplazo,
                IdLlantaAsignacionDestino = asignacionEntrante.IdLlantaAsignacion,
                IdVehiculoDestino = (ulong)asignacionEntrante.IdVehiculo,
                IdVehiculoEjeDestino = (ulong)asignacionEntrante.IdVehiculoEje,
                PosicionDestino = asignacionEntrante.NumeroPosicion,
                Kilometraje = model.KmVehiculo,
                CreadoPor = (uint)idUsuario
            });

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        public async Task<bool> RotarLlanta(LlantaRotacionViewModel model, int idEmpresa, int idUsuario)
        {
            if (model.IdLlantaAsignacionOrigen == 0)
                throw new Exception("La asignación origen es obligatoria.");

            if (model.IdVehiculoEjeDestino <= 0)
                throw new Exception("El eje destino es obligatorio.");

            if (model.NumeroPosicionDestino <= 0)
                throw new Exception("La posición destino es obligatoria.");

            if (model.FechaMovimiento == default)
                throw new Exception("La fecha de rotación es obligatoria.");

            if (model.Observaciones?.Length > 500)
                throw new Exception("Las observaciones no deben exceder 500 caracteres.");

            if (model.FechaMovimiento.Date > DateTime.Today)
                throw new Exception("La fecha de rotación no puede ser posterior a la fecha actual.");

            await using var transaction = await context.Database.BeginTransactionAsync();

            var asignacionOrigen = await context.LlantaAsignacions
                .Include(x => x.IdVehiculoNavigation)
                .FirstOrDefaultAsync(x => x.IdLlantaAsignacion == model.IdLlantaAsignacionOrigen
                                       && x.Activa == true);

            if (asignacionOrigen == null)
                throw new Exception("No se encontró una asignación activa para rotar.");

            if (asignacionOrigen.IdVehiculoNavigation.EmpresaIdempresa != idEmpresa)
                throw new Exception("La asignación no pertenece a su empresa.");

            if (asignacionOrigen.IdVehiculoEje == model.IdVehiculoEjeDestino
                && asignacionOrigen.NumeroPosicion == model.NumeroPosicionDestino)
                throw new Exception("La posición destino debe ser distinta a la posición origen.");

            if (model.FechaMovimiento < asignacionOrigen.FechaAsignacion)
                throw new Exception("La fecha de rotación no puede ser anterior a la fecha de instalación de la llanta origen.");

            var ejeDestino = await context.VehiculoEjes
                .Include(x => x.IdTipoEjeNavigation)
                .FirstOrDefaultAsync(x => x.IdVehiculoEje == model.IdVehiculoEjeDestino
                                       && x.IdVehiculo == asignacionOrigen.IdVehiculo
                                       && x.Activo == true);

            if (ejeDestino == null)
                throw new Exception("No se encontró el eje destino o no pertenece al vehículo seleccionado.");

            if (model.NumeroPosicionDestino > ejeDestino.IdTipoEjeNavigation.NumeroPosiciones)
                throw new Exception("La posición destino no existe para el tipo de eje seleccionado.");

            var existeTipoRotacion = await context.LlantaTipoAsignacions
                .AsNoTracking()
                .AnyAsync(x => x.IdTipoAsignacion == TipoAsignacionRotacion && x.EsActivo == true);

            if (!existeTipoRotacion)
                throw new Exception("No se encontró el tipo de asignación Rotación activo con id 3.");

            var asignacionDestino = await context.LlantaAsignacions
                .FirstOrDefaultAsync(x => x.IdVehiculo == asignacionOrigen.IdVehiculo
                                       && x.IdVehiculoEje == model.IdVehiculoEjeDestino
                                       && x.NumeroPosicion == model.NumeroPosicionDestino
                                       && x.Activa == true);

            if (asignacionDestino != null && model.FechaMovimiento < asignacionDestino.FechaAsignacion)
                throw new Exception("La fecha de rotación no puede ser anterior a la fecha de instalación de la llanta destino.");

            var ultimoKmValido = await GetUltimoKilometrajeValido(asignacionOrigen.IdVehiculo);
            if (model.KmVehiculo < ultimoKmValido)
                throw new Exception($"El kilometraje no puede ser menor al último kilometraje registrado ({ultimoKmValido:0}).");

            if (model.KmVehiculo < asignacionOrigen.KmVehiculoAsignacion)
                throw new Exception($"El kilometraje de rotación no puede ser menor al kilometraje de instalación de la llanta origen ({asignacionOrigen.KmVehiculoAsignacion}).");

            if (asignacionDestino != null && model.KmVehiculo < asignacionDestino.KmVehiculoAsignacion)
                throw new Exception($"El kilometraje de rotación no puede ser menor al kilometraje de instalación de la llanta destino ({asignacionDestino.KmVehiculoAsignacion}).");

            var observaciones = string.IsNullOrWhiteSpace(model.Observaciones) ? null : model.Observaciones.Trim();
            var fechaAhora = DateTime.Now;
            var usuario = (uint)idUsuario;

            asignacionOrigen.FechaRetiro = model.FechaMovimiento;
            asignacionOrigen.KmVehiculoRetiro = model.KmVehiculo;
            asignacionOrigen.IdMotivoRetiro = null;
            asignacionOrigen.ObservacionesRetiro = observaciones;
            asignacionOrigen.Activa = false;
            asignacionOrigen.FechaModificacion = fechaAhora;
            asignacionOrigen.ModificadoPor = usuario;

            LlantaAsignacion? nuevaAsignacionDestinoOrigen = null;

            if (asignacionDestino != null)
            {
                asignacionDestino.FechaRetiro = model.FechaMovimiento;
                asignacionDestino.KmVehiculoRetiro = model.KmVehiculo;
                asignacionDestino.IdMotivoRetiro = null;
                asignacionDestino.ObservacionesRetiro = observaciones;
                asignacionDestino.Activa = false;
                asignacionDestino.FechaModificacion = fechaAhora;
                asignacionDestino.ModificadoPor = usuario;

                nuevaAsignacionDestinoOrigen = CrearAsignacionRotacion(
                    asignacionDestino.IdLlanta,
                    asignacionOrigen.IdVehiculo,
                    asignacionOrigen.IdVehiculoEje,
                    asignacionOrigen.NumeroPosicion,
                    model.FechaMovimiento,
                    model.KmVehiculo,
                    observaciones,
                    fechaAhora,
                    usuario);
            }

            var nuevaAsignacionOrigenDestino = CrearAsignacionRotacion(
                asignacionOrigen.IdLlanta,
                asignacionOrigen.IdVehiculo,
                model.IdVehiculoEjeDestino,
                model.NumeroPosicionDestino,
                model.FechaMovimiento,
                model.KmVehiculo,
                observaciones,
                fechaAhora,
                usuario);

            await context.SaveChangesAsync();
            context.LlantaAsignacions.Add(nuevaAsignacionOrigenDestino);

            if (nuevaAsignacionDestinoOrigen != null)
            {
                context.LlantaAsignacions.Add(nuevaAsignacionDestinoOrigen);
            }

            await context.SaveChangesAsync();

            var idOperacionRotacion = asignacionDestino != null ? Guid.NewGuid() : (Guid?)null;
            var fechaMovimientoBitacora = DateTime.Now;

            var bitacoraOrigen = RegistrarMovimientoBitacora(new LlantaAsignacionBitacoraMovimiento
            {
                IdOperacion = idOperacionRotacion,
                IdLlanta = asignacionOrigen.IdLlanta,
                TipoMovimiento = LlantaMovimientoBitacoraTipo.Rotacion,
                IdLlantaAsignacionOrigen = asignacionOrigen.IdLlantaAsignacion,
                IdVehiculoOrigen = (ulong)asignacionOrigen.IdVehiculo,
                IdVehiculoEjeOrigen = (ulong)asignacionOrigen.IdVehiculoEje,
                PosicionOrigen = asignacionOrigen.NumeroPosicion,
                IdLlantaAsignacionDestino = nuevaAsignacionOrigenDestino.IdLlantaAsignacion,
                IdVehiculoDestino = (ulong)nuevaAsignacionOrigenDestino.IdVehiculo,
                IdVehiculoEjeDestino = (ulong)nuevaAsignacionOrigenDestino.IdVehiculoEje,
                PosicionDestino = nuevaAsignacionOrigenDestino.NumeroPosicion,
                Kilometraje = model.KmVehiculo,
                CreadoPor = (uint)idUsuario
            });
            bitacoraOrigen.FechaMovimiento = fechaMovimientoBitacora;

            if (asignacionDestino != null && nuevaAsignacionDestinoOrigen != null)
            {
                var bitacoraDestino = RegistrarMovimientoBitacora(new LlantaAsignacionBitacoraMovimiento
                {
                    IdOperacion = idOperacionRotacion,
                    IdLlanta = asignacionDestino.IdLlanta,
                    TipoMovimiento = LlantaMovimientoBitacoraTipo.Rotacion,
                    IdLlantaAsignacionOrigen = asignacionDestino.IdLlantaAsignacion,
                    IdVehiculoOrigen = (ulong)asignacionDestino.IdVehiculo,
                    IdVehiculoEjeOrigen = (ulong)asignacionDestino.IdVehiculoEje,
                    PosicionOrigen = asignacionDestino.NumeroPosicion,
                    IdLlantaAsignacionDestino = nuevaAsignacionDestinoOrigen.IdLlantaAsignacion,
                    IdVehiculoDestino = (ulong)nuevaAsignacionDestinoOrigen.IdVehiculo,
                    IdVehiculoEjeDestino = (ulong)nuevaAsignacionDestinoOrigen.IdVehiculoEje,
                    PosicionDestino = nuevaAsignacionDestinoOrigen.NumeroPosicion,
                    Kilometraje = model.KmVehiculo,
                    CreadoPor = (uint)idUsuario
                });
                bitacoraDestino.FechaMovimiento = fechaMovimientoBitacora;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        private static LlantaAsignacion CrearAsignacionRotacion(
            ulong idLlanta,
            int idVehiculo,
            int idVehiculoEje,
            ushort numeroPosicion,
            DateTime fechaMovimiento,
            uint kmVehiculo,
            string? observaciones,
            DateTime fechaAhora,
            uint usuario)
        {
            return new LlantaAsignacion
            {
                IdLlanta = idLlanta,
                IdVehiculo = idVehiculo,
                IdVehiculoEje = idVehiculoEje,
                NumeroPosicion = numeroPosicion,
                IdTipoAsignacion = TipoAsignacionRotacion,
                FechaAsignacion = fechaMovimiento,
                KmVehiculoAsignacion = kmVehiculo,
                FechaRetiro = null,
                KmVehiculoRetiro = null,
                IdMotivoRetiro = null,
                ObservacionesAsignacion = observaciones,
                ObservacionesRetiro = null,
                Activa = true,
                FechaCreacion = fechaAhora,
                CreadoPor = usuario,
                FechaModificacion = null,
                ModificadoPor = null,
                FechaEliminacion = null,
                EliminadoPor = null
            };
        }

        private LlantaAsignacionBitacora RegistrarMovimientoBitacora(LlantaAsignacionBitacoraMovimiento movimiento)
        {
            if (movimiento == null)
                throw new ArgumentNullException(nameof(movimiento));

            if (movimiento.IdLlanta == 0)
                throw new Exception("La llanta es obligatoria para registrar la bitácora.");

            if (!LlantaMovimientoBitacoraTipo.EsValido(movimiento.TipoMovimiento))
                throw new Exception("El tipo de movimiento de bitácora no es válido.");

            if (movimiento.CreadoPor == 0)
                throw new Exception("El usuario que registra la bitácora es obligatorio.");

            if (movimiento.Observaciones?.Length > 500)
                throw new Exception("Las observaciones de bitácora no deben exceder 500 caracteres.");

            var bitacora = new LlantaAsignacionBitacora
            {
                IdOperacion = movimiento.IdOperacion,
                IdLlanta = movimiento.IdLlanta,
                TipoMovimiento = LlantaMovimientoBitacoraTipo.Normalizar(movimiento.TipoMovimiento),
                IdLlantaAsignacionOrigen = movimiento.IdLlantaAsignacionOrigen,
                IdVehiculoOrigen = movimiento.IdVehiculoOrigen,
                IdVehiculoEjeOrigen = movimiento.IdVehiculoEjeOrigen,
                PosicionOrigen = movimiento.PosicionOrigen,
                IdLlantaAsignacionDestino = movimiento.IdLlantaAsignacionDestino,
                IdVehiculoDestino = movimiento.IdVehiculoDestino,
                IdVehiculoEjeDestino = movimiento.IdVehiculoEjeDestino,
                PosicionDestino = movimiento.PosicionDestino,
                Kilometraje = movimiento.Kilometraje,
                Observaciones = string.IsNullOrWhiteSpace(movimiento.Observaciones) ? null : movimiento.Observaciones.Trim(),
                CreadoPor = movimiento.CreadoPor,
                FechaMovimiento = DateTime.Now
            };

            context.LlantaAsignacionBitacoras.Add(bitacora);

            return bitacora;
        }

        private async Task<decimal> GetUltimoKilometrajeValido(int idVehiculo)
        {
            var kilometrajesVehiculo = await context.VehiculoDetalles
                .AsNoTracking()
                .Where(x => x.IdVehiculo == idVehiculo && x.KilometrajeAcumulado.HasValue)
                .Select(x => x.KilometrajeAcumulado!.Value)
                .ToListAsync();

            var kilometrajesAsignacion = await context.LlantaAsignacions
                .AsNoTracking()
                .Where(x => x.IdVehiculo == idVehiculo)
                .Select(x => new { x.KmVehiculoAsignacion, x.KmVehiculoRetiro })
                .ToListAsync();

            var ultimoKmVehiculo = kilometrajesVehiculo.Any()
                ? kilometrajesVehiculo.Max()
                : 0;
            var ultimoKmAsignacion = kilometrajesAsignacion
                .SelectMany(x => new[] { (uint?)x.KmVehiculoAsignacion, x.KmVehiculoRetiro })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .DefaultIfEmpty((uint)0)
                .Max();

            return Math.Max((decimal)ultimoKmVehiculo, (decimal)ultimoKmAsignacion);
        }

        private async Task<ushort> GetIdEstadoInstalada()
        {
            var estados = await context.LlantaEstados
                .AsNoTracking()
                .Where(x => x.EsActivo == true)
                .Select(x => new { x.IdEstadoLlanta, x.Nombre })
                .ToListAsync();

            var idEstadoInstalada = estados
                .FirstOrDefault(x => NormalizarTexto(x.Nombre).Contains("instalada"))?.IdEstadoLlanta;

            if (!idEstadoInstalada.HasValue)
                throw new Exception("No se encontró el estado de llanta Instalada.");

            return idEstadoInstalada.Value;
        }

        private static bool EstadoPermiteInstalacion(string? estado)
        {
            var nombre = NormalizarTexto(estado);
            return !nombre.Contains("instalada")
                && !nombre.Contains("inspeccion")
                && !nombre.Contains("reparacion")
                && !nombre.Contains("renovado")
                && !nombre.Contains("fuera")
                && !nombre.Contains("baja");
        }

        private static string NormalizarTexto(string? value)
        {
            return (value ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("ü", "u");
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

        private async Task ValidarDuplicados(LlantaCrearViewModel model)
        {
            var codigo = model.CodigoLlanta.Trim().ToLower();
            var serie = model.NumeroSerieDot.Trim().ToLower();

            bool codigoDuplicado = await context.Llanta
                .AnyAsync(x => !x.Eliminado
                            && x.IdLlanta != model.IdLlanta
                            && x.CodigoLlanta.Trim().ToLower() == codigo);
            if (codigoDuplicado)
                throw new Exception("Ya existe una llanta con el mismo código.");

            bool serieDuplicada = await context.Llanta
                .AnyAsync(x => !x.Eliminado
                            && x.IdLlanta != model.IdLlanta
                            && x.NumeroSerieDot.Trim().ToLower() == serie);
            if (serieDuplicada)
                throw new Exception("Ya existe una llanta con el mismo número de serie / DOT.");
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
