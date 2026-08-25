using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using System.Globalization;
using System.Text;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class LlantaInspeccionService : ILlantaInspeccionService
    {
        private static readonly HashSet<string> ConclusionesConAccion = new(StringComparer.OrdinalIgnoreCase)
        {
            "renovar",
            "reparar",
            "desechar"
        };

        private const string ContextoFueraVehiculo = "FueraVehiculo";

        private readonly SuvanDbContext context;
        private readonly ILlantaService llantaService;

        public LlantaInspeccionService(SuvanDbContext context, ILlantaService llantaService)
        {
            this.context = context;
            this.llantaService = llantaService;
        }

        public async Task<LlantaInspeccionViewModel> GetViewModel(int idEmpresa)
        {
            return new LlantaInspeccionViewModel
            {
                Vehiculos = (await llantaService.GetVehiculosParaAsignacion(idEmpresa))
                    .Select(x => new LlantaInspeccionCatalogoViewModel
                    {
                        Id = (ulong)x.Id,
                        Nombre = x.Nombre ?? string.Empty
                    })
                    .ToList(),
                TiposInspeccion = await context.LlantaTipoInspeccions
                    .AsNoTracking()
                    .Where(x => x.EsActivo == true)
                    .OrderBy(x => x.IdTipoInspeccion)
                    .Select(x => new LlantaInspeccionCatalogoViewModel
                    {
                        Id = x.IdTipoInspeccion,
                        Nombre = x.Nombre
                    })
                    .ToListAsync(),
                EstadosInspeccion = await context.LlantaEstadoInspeccions
                    .AsNoTracking()
                    .Where(x => x.EsActivo == true)
                    .OrderBy(x => x.IdEstadoInspeccion)
                    .Select(x => new LlantaInspeccionCatalogoViewModel
                    {
                        Id = x.IdEstadoInspeccion,
                        Nombre = x.Nombre
                    })
                    .ToListAsync(),
                ConclusionesInspeccion = await context.LlantaConclusionInspeccions
                    .AsNoTracking()
                    .Where(x => x.EsActivo == true)
                    .OrderBy(x => x.IdConclusionInspeccion)
                    .Select(x => new LlantaInspeccionCatalogoViewModel
                    {
                        Id = x.IdConclusionInspeccion,
                        Nombre = x.Nombre
                    })
                    .ToListAsync(),
                EstadosLlanta = await context.LlantaEstados
                    .AsNoTracking()
                    .Where(x => x.EsActivo == true)
                    .OrderBy(x => x.IdEstadoLlanta)
                    .Select(x => new LlantaInspeccionCatalogoViewModel
                    {
                        Id = x.IdEstadoLlanta,
                        Nombre = x.Nombre
                    })
                    .ToListAsync()
            };
        }

        public async Task<LlantaConfiguracionVehiculoViewModel> GetConfiguracionVehiculo(int idVehiculo, int idEmpresa)
        {
            return await llantaService.GetConfiguracionVehiculoLlantas(idVehiculo, idEmpresa);
        }

        public async Task<List<LlantaInspeccionLlantaFueraVehiculoViewModel>> GetLlantasFueraVehiculo(int idEmpresa)
        {
            var estadosExcluidos = await context.LlantaEstados
                .AsNoTracking()
                .Where(x => x.EsActivo == true)
                .Select(x => new { x.IdEstadoLlanta, x.Nombre })
                .ToListAsync();

            var idsEstadosExcluidos = estadosExcluidos
                .Where(x =>
                {
                    var nombre = NormalizarTexto(x.Nombre);
                    return nombre.Contains("instalada") || nombre.Contains("baja definitiva");
                })
                .Select(x => x.IdEstadoLlanta)
                .ToList();

            return await (
                    from llanta in context.Llanta.AsNoTracking()
                    join deposito in context.Depositos.AsNoTracking()
                        on llanta.IdDeposito equals (uint?)deposito.IdDeposito into depositoGroup
                    from deposito in depositoGroup.DefaultIfEmpty()
                    where !llanta.Eliminado
                          && llanta.IdEmpresa == (uint)idEmpresa
                          && !idsEstadosExcluidos.Contains(llanta.IdEstadoLlanta)
                          && !context.LlantaAsignacions.Any(asignacion => asignacion.IdLlanta == llanta.IdLlanta && asignacion.Activa == true)
                    orderby llanta.CodigoLlanta
                    select new LlantaInspeccionLlantaFueraVehiculoViewModel
                    {
                        IdLlanta = llanta.IdLlanta,
                        CodigoLlanta = llanta.CodigoLlanta,
                        NumeroSerieDot = llanta.NumeroSerieDot,
                        Marca = llanta.IdModeloLlantaNavigation.IdMarcaLlantaNavigation.Nombre,
                        Modelo = llanta.IdModeloLlantaNavigation.Nombre,
                        Medida = llanta.IdModeloLlantaNavigation.Medida,
                        IdEstadoLlanta = llanta.IdEstadoLlanta,
                        EstadoLlanta = llanta.IdEstadoLlantaNavigation.Nombre,
                        IdDeposito = llanta.IdDeposito,
                        Deposito = deposito != null ? deposito.NombreDeposito : null,
                        PresionMinimaPsi = llanta.IdModeloLlantaNavigation.PresionMinimaPsi,
                        PresionMaximaPsi = llanta.IdModeloLlantaNavigation.PresionMaximaPsi,
                        ProfundidadOriginalMm = llanta.IdModeloLlantaNavigation.ProfundidadOriginalMm,
                        ProfundidadAlertaMm = llanta.IdModeloLlantaNavigation.ProfundidadAlertaMm,
                        ProfundidadMinimaMm = llanta.IdModeloLlantaNavigation.ProfundidadMinimaMm,
                        VidaUtilEstimadaKm = llanta.IdModeloLlantaNavigation.VidaUtilEstimadaKm
                    })
                .ToListAsync();
        }

        public async Task<bool> GuardarInspeccion(LlantaInspeccionGuardarViewModel model, int idEmpresa, int idUsuario)
        {
            var esFueraVehiculo = EsContextoFueraVehiculo(model.ContextoInspeccion);

            if (!esFueraVehiculo && model.IdVehiculo <= 0)
                throw new Exception("El vehículo es obligatorio.");

            if (model.IdTipoInspeccion == 0)
                throw new Exception("El tipo de inspección es obligatorio.");

            if (model.FechaInspeccion == default)
                throw new Exception("La fecha de inspección es obligatoria.");

            if (model.FechaInspeccion.Date > DateTime.Today)
                throw new Exception("La fecha de inspección no puede ser posterior a la fecha actual.");

            if (model.Detalles == null || !model.Detalles.Any())
                throw new Exception("Selecciona al menos una llanta para inspeccionar.");

            if (!esFueraVehiculo && model.Detalles.Any(x => x.IdLlantaAsignacion == 0))
                throw new Exception("Todas las llantas seleccionadas deben tener una asignación activa.");

            if (esFueraVehiculo)
            {
                if (model.Detalles.Any(x => x.IdLlanta == 0))
                    throw new Exception("Todas las llantas seleccionadas son obligatorias.");

                if (model.Detalles.Select(x => x.IdLlanta).Distinct().Count() != model.Detalles.Count)
                    throw new Exception("No se puede registrar la misma llanta más de una vez en la inspección.");
            }
            else if (model.Detalles.Select(x => x.IdLlantaAsignacion).Distinct().Count() != model.Detalles.Count)
            {
                throw new Exception("No se puede registrar la misma llanta más de una vez en la inspección.");
            }

            foreach (var detalle in model.Detalles)
            {
                if (detalle.ProfundidadMm.HasValue && detalle.ProfundidadMm < 0)
                    throw new Exception("La profundidad no puede ser negativa.");

                if (detalle.PresionPsi.HasValue && detalle.PresionPsi < 0)
                    throw new Exception("La presión no puede ser negativa.");

                if (detalle.IdEstadoInspeccion == 0)
                    throw new Exception("El estado de inspección es obligatorio para cada llanta.");

                if (detalle.IdConclusionInspeccion == 0)
                    throw new Exception("La conclusión es obligatoria para cada llanta.");

                if (detalle.Observaciones?.Length > 1000)
                    throw new Exception("Las observaciones no deben exceder 1000 caracteres.");
            }

            await using var transaction = await context.Database.BeginTransactionAsync();

            if (!esFueraVehiculo)
            {
                var vehiculoExiste = await context.Vehiculos
                    .AnyAsync(x => x.IdVehiculo == model.IdVehiculo && x.EmpresaIdempresa == idEmpresa);

                if (!vehiculoExiste)
                    throw new Exception("No se encontró el vehículo o no pertenece a su empresa.");
            }

            var tipoValido = await context.LlantaTipoInspeccions
                .AsNoTracking()
                .AnyAsync(x => x.IdTipoInspeccion == model.IdTipoInspeccion && x.EsActivo == true);

            if (!tipoValido)
                throw new Exception("El tipo de inspección no está activo o no existe.");

            var idsEstado = model.Detalles.Select(x => x.IdEstadoInspeccion).Distinct().ToList();
            var estadosValidos = await context.LlantaEstadoInspeccions
                .AsNoTracking()
                .CountAsync(x => idsEstado.Contains(x.IdEstadoInspeccion) && x.EsActivo == true);

            if (estadosValidos != idsEstado.Count)
                throw new Exception("Uno o más estados de inspección no están activos o no existen.");

            var idsConclusion = model.Detalles.Select(x => x.IdConclusionInspeccion).Distinct().ToList();
            var conclusiones = await context.LlantaConclusionInspeccions
                .AsNoTracking()
                .Where(x => idsConclusion.Contains(x.IdConclusionInspeccion) && x.EsActivo == true)
                .Select(x => new { x.IdConclusionInspeccion, x.Nombre })
                .ToListAsync();

            if (conclusiones.Count != idsConclusion.Count)
                throw new Exception("Una o más conclusiones de inspección no están activas o no existen.");

            var conclusionesPorId = conclusiones.ToDictionary(x => x.IdConclusionInspeccion, x => x.Nombre);
            var asignacionesPorId = new Dictionary<ulong, ulong>();
            var llantasFueraVehiculoPorId = new Dictionary<ulong, ulong>();

            if (esFueraVehiculo)
            {
                var idsLlanta = model.Detalles.Select(x => x.IdLlanta).ToList();
                var llantasFueraVehiculo = await context.Llanta
                    .AsNoTracking()
                    .Where(x => idsLlanta.Contains(x.IdLlanta)
                             && x.IdEmpresa == (uint)idEmpresa
                             && !x.Eliminado
                             && !context.LlantaAsignacions.Any(asignacion => asignacion.IdLlanta == x.IdLlanta && asignacion.Activa == true))
                    .Select(x => x.IdLlanta)
                    .ToListAsync();

                if (llantasFueraVehiculo.Count != idsLlanta.Count)
                    throw new Exception("Una o más llantas seleccionadas no están disponibles para inspección fuera de vehículo.");

                llantasFueraVehiculoPorId = llantasFueraVehiculo.ToDictionary(x => x);
            }
            else
            {
                var idsAsignacion = model.Detalles.Select(x => x.IdLlantaAsignacion).ToList();
                var asignaciones = await context.LlantaAsignacions
                    .AsNoTracking()
                    .Include(x => x.IdVehiculoNavigation)
                    .Where(x => idsAsignacion.Contains(x.IdLlantaAsignacion)
                             && x.IdVehiculo == model.IdVehiculo
                             && x.IdVehiculoNavigation.EmpresaIdempresa == idEmpresa
                             && x.Activa == true)
                    .Select(x => new
                    {
                        x.IdLlantaAsignacion,
                        x.IdLlanta
                    })
                    .ToListAsync();

                if (asignaciones.Count != idsAsignacion.Count)
                    throw new Exception("Una o más llantas seleccionadas ya no tienen una asignación activa en el vehículo.");

                asignacionesPorId = asignaciones.ToDictionary(x => x.IdLlantaAsignacion, x => x.IdLlanta);
            }
            var ahora = DateTime.Now;

            foreach (var detalle in model.Detalles)
            {
                var conclusion = conclusionesPorId[detalle.IdConclusionInspeccion];
                var idLlanta = esFueraVehiculo
                    ? llantasFueraVehiculoPorId[detalle.IdLlanta]
                    : asignacionesPorId[detalle.IdLlantaAsignacion];

                context.LlantaInspeccions.Add(new LlantaInspeccion
                {
                    IdLlanta = idLlanta,
                    IdTipoInspeccion = model.IdTipoInspeccion,
                    IdLlantaAsignacion = esFueraVehiculo ? null : detalle.IdLlantaAsignacion,
                    FechaInspeccion = model.FechaInspeccion,
                    KilometrajeLlanta = model.KilometrajeLlanta,
                    ProfundidadMm = detalle.ProfundidadMm,
                    PresionPsi = detalle.PresionPsi,
                    IdEstadoInspeccion = detalle.IdEstadoInspeccion,
                    IdConclusionInspeccion = detalle.IdConclusionInspeccion,
                    RequiereAccion = RequiereAccion(conclusion),
                    Observaciones = string.IsNullOrWhiteSpace(detalle.Observaciones) ? null : detalle.Observaciones.Trim(),
                    FechaCreacion = ahora,
                    CreadoPor = (uint)idUsuario
                });
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        private static bool RequiereAccion(string conclusion)
        {
            return ConclusionesConAccion.Contains(NormalizarTexto(conclusion));
        }

        private static bool EsContextoFueraVehiculo(string? contexto)
        {
            return string.Equals(contexto, ContextoFueraVehiculo, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizarTexto(string value)
        {
            var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                    builder.Append(character);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
