using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
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
            var modelo = await context.Modelos.FirstOrDefaultAsync(x => x.IdModelo == id);

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
                    TipoEje = modelo.TipoEje,

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
        public async Task<bool> AgregarModelo(ModeloViewModel model)
        {
            Modelo modelos;

            if (model.IdModelo > 0)
            {
                modelos = await context.Modelos.FirstOrDefaultAsync(x => x.IdModelo == model.IdModelo);

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

            if (model.IdModelo > 0)
            {
                context.Modelos.Entry(modelos);

                await context.SaveChangesAsync();
            }
            else
            {
                context.Modelos.Add(modelos);
                await context.SaveChangesAsync();

            }
            return true;
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
    }
}