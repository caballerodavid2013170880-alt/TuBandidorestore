using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class ModeloService : IModeloService
    {
        private readonly SuvanDbContext context;

        public ModeloService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Modelo>> GetModelo()
        {
            var modelo = await context.Modelos.Include(m => m.IdMarcaNavigation).Include(t => t.IdTipoVNavigation).ToListAsync();

            return modelo!;
        }


        /// <summary>
        /// Obtiene el ViewModel del Modelo específico.
        /// </summary>
        /// <param name="id">Identificador del modelo.</param>
        /// <returns>ViewModel para el modelo especifico.</returns>
        public async Task<ModeloViewModel> GetModeloViewModel(int id)
        {
            ModeloViewModel vRet = new ModeloViewModel();
            var modelo = await context.Modelos
                .Include(x => x.ModeloEjes.Where(e => e.EsActivo == true))
                    .ThenInclude(x => x.IdTipoEjeNavigation)
                .FirstOrDefaultAsync(x => x.IdModelo == id);

            if (modelo == null)
                return vRet;
            else
            {
                vRet = new ModeloViewModel
                {
                    IdModelo = modelo.IdModelo,
                    IdMarca = modelo.IdMarca,
                    IdTipoV = modelo.IdTipoV,
                    AnioDesde = modelo.AnioDesde,
                    AnioHasta = modelo.AnioHasta,
                    Descripcion = modelo.Descripcion,
                    KmGarantia = modelo.KmGarantia,
                    MesGarantia = modelo.MesGarantia,
                    Ejes = modelo.ModeloEjes
                        .Where(x => x.EsActivo == true)
                        .OrderBy(x => x.NumeroEje)
                        .Select(x => new ModeloEjeViewModel
                        {
                            IdModeloEje = x.IdModeloEje,
                            IdTipoEje = x.IdTipoEje,
                            NumeroEje = x.NumeroEje,
                            NumeroPosiciones = x.IdTipoEjeNavigation.NumeroPosiciones
                        })
                        .ToList()
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza una modelo en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del modelo.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarModelo(ModeloViewModel model, int idUsuario)
        {
            ValidarModelo(model);

            await using var transaction = await context.Database.BeginTransactionAsync();

            Modelo modelos;
            var fechaActual = DateTime.Now;
            int? usuarioAuditoria = idUsuario > 0 ? idUsuario : null;

            try
            {
                if (model.IdModelo > 0)
                {
                    modelos = await context.Modelos
                        .Include(x => x.ModeloEjes)
                        .FirstOrDefaultAsync(x => x.IdModelo == model.IdModelo);

                    if (modelos == null)
                        throw new Exception("No se encontro el Modelo");

                }
                else
                {
                    modelos = new Modelo();
                }

                // Valida si la descripción del modelo esta duplicado
                var modeloExistenteDescripcion = await context.Modelos.FirstOrDefaultAsync(x =>
                x.Descripcion!.ToLower() == model.Descripcion!.ToLower()
                && x.IdModelo != model.IdModelo);

                if (modeloExistenteDescripcion is not null)
                    throw new Exception("Ya existe un Modelo con la misma descripción");

                modelos.Descripcion = model.Descripcion;
                modelos.IdMarca = model.IdMarca;
                modelos.IdTipoV = model.IdTipoV;
                modelos.AnioDesde = model.AnioDesde;
                modelos.AnioHasta = model.AnioHasta;
                modelos.MesGarantia = model.MesGarantia;
                modelos.KmGarantia = model.KmGarantia;

                if (model.IdModelo == 0)
                {
                    context.Modelos.Add(modelos);
                    await context.SaveChangesAsync();
                }

                await GuardarEjesModelo(modelos, model.Ejes, fechaActual, usuarioAuditoria);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Elimina un modelo en la base de datos.
        /// </summary>
        /// <param name="IdModelo">Identificador del modelo.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarModelo(int IdModelo)
        {
            var modelo = await context.Modelos.FirstOrDefaultAsync(x => x.IdModelo == IdModelo);

            if (modelo is null)
            {
                throw new Exception("No se encontro el Modelo");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.Modelos
              .Where(x => x.IdModelo == IdModelo)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }

        public List<MarcaViewModel> ObtenerMarca()
        {
            var resul = (from o in context.Marcas select new MarcaViewModel()
                         {
                             IdMarca = o.IdMarca,
                             Descripcion = o.Descripcion
                         }).ToList();

            return resul;
        }

        public List<TipoVehiculoViewModel> ObtenerTipoVehiculo()
        {
            var resul = (from o in context.Tipovehiculos select new TipoVehiculoViewModel()
                         {
                             TipoUnidadId = o.Idtipovehiculo,
                             Nombre = o.Nombre
                         }).ToList();

            return resul;
        }

        public List<TipoEjeCatalogoViewModel> ObtenerTipoEje()
        {
            return context.TipoEjes
                .Where(x => x.EsActivo == true)
                .OrderBy(x => x.Nombre)
                .Select(x => new TipoEjeCatalogoViewModel
                {
                    IdTipoEje = x.IdTipoEje,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    NumeroPosiciones = x.NumeroPosiciones
                })
                .ToList();
        }

        private void ValidarModelo(ModeloViewModel model)
        {
            if (model.IdMarca <= 0)
                throw new Exception("Marca requerida");

            if (model.IdTipoV <= 0)
                throw new Exception("Tipo de Vehículo requerido");

            if (string.IsNullOrWhiteSpace(model.Descripcion))
                throw new Exception("Descripción requerida");

            if (model.AnioDesde <= 0)
                throw new Exception("Año Desde requerido");

            if (model.AnioHasta <= 0)
                throw new Exception("Año Hasta requerido");

            if (model.AnioHasta < model.AnioDesde)
                throw new Exception("Año Hasta no puede ser menor que Año Desde");

            if (model.KmGarantia <= 0)
                throw new Exception("Kilómetros de Garantía requerido");

            if (model.MesGarantia <= 0)
                throw new Exception("Meses de Garantía requerido");

            if (model.Ejes == null || !model.Ejes.Any())
                throw new Exception("Debe configurar al menos un eje");

            var tiposEje = context.TipoEjes
                .Where(x => x.EsActivo == true)
                .Select(x => x.IdTipoEje)
                .ToHashSet();

            foreach (var eje in model.Ejes)
            {
                if (eje.IdTipoEje <= 0)
                    throw new Exception("Todos los ejes deben tener un tipo de eje seleccionado");

                if (!tiposEje.Contains(eje.IdTipoEje))
                    throw new Exception("El tipo de eje seleccionado no está activo o no existe");
            }

            var idsExistentes = model.Ejes
                .Where(x => x.IdModeloEje > 0)
                .Select(x => x.IdModeloEje)
                .ToList();

            if (idsExistentes.Count != idsExistentes.Distinct().Count())
                throw new Exception("Existe un eje duplicado en la configuración");
        }

        private async Task GuardarEjesModelo(Modelo modelo, List<ModeloEjeViewModel> ejes, DateTime fechaActual, int? usuarioAuditoria)
        {
            var ejesActuales = modelo.ModeloEjes.ToList();

            if (ejesActuales.Any())
            {
                ushort numeroTemporal = 50000;

                foreach (var ejeActual in ejesActuales)
                {
                    ejeActual.NumeroEje = numeroTemporal++;
                }

                await context.SaveChangesAsync();
            }

            var idsEnviados = ejes
                .Where(x => x.IdModeloEje > 0)
                .Select(x => x.IdModeloEje)
                .ToHashSet();

            var ejesEliminados = ejesActuales
                .Where(x => x.EsActivo == true && !idsEnviados.Contains(x.IdModeloEje))
                .ToList();

            foreach (var ejeEliminado in ejesEliminados)
            {
                ejeEliminado.EsActivo = false;
                ejeEliminado.FechaEliminacion = fechaActual;
                ejeEliminado.EliminadoPor = usuarioAuditoria;
                ejeEliminado.FechaModificacion = fechaActual;
                ejeEliminado.ModificadoPor = usuarioAuditoria;
            }

            for (var index = 0; index < ejes.Count; index++)
            {
                var ejeModel = ejes[index];
                var numeroEje = (ushort)(index + 1);

                if (ejeModel.IdModeloEje > 0)
                {
                    var ejeExistente = ejesActuales.FirstOrDefault(x => x.IdModeloEje == ejeModel.IdModeloEje);

                    if (ejeExistente == null || ejeExistente.IdModelo != modelo.IdModelo)
                        throw new Exception("La configuración de ejes contiene un registro inválido");

                    if (ejeExistente.EsActivo != true)
                        throw new Exception("La configuración de ejes contiene un registro inactivo");

                    ejeExistente.IdTipoEje = ejeModel.IdTipoEje;
                    ejeExistente.NumeroEje = numeroEje;
                    ejeExistente.EsActivo = true;
                    ejeExistente.FechaModificacion = fechaActual;
                    ejeExistente.ModificadoPor = usuarioAuditoria;
                    ejeExistente.FechaEliminacion = null;
                    ejeExistente.EliminadoPor = null;
                }
                else
                {
                    context.ModeloEjes.Add(new ModeloEje
                    {
                        IdModelo = modelo.IdModelo,
                        IdTipoEje = ejeModel.IdTipoEje,
                        NumeroEje = numeroEje,
                        EsActivo = true,
                        FechaCreacion = fechaActual,
                        CreadoPor = usuarioAuditoria,
                        FechaModificacion = null,
                        ModificadoPor = null,
                        FechaEliminacion = null,
                        EliminadoPor = null
                    });
                }
            }
        }
    }
}
