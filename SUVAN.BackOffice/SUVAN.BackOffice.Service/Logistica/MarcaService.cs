using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class MarcaService : IMarcaService
    {
        private readonly SuvanDbContext context;

        public MarcaService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Marca>> GetMarca()
        {

            var marca = await context.Marcas.ToListAsync();

            return marca!;
        }

        /// <summary>
        /// Obtiene el ViewModel de la marca específica.
        /// </summary>
        /// <param name="id">Identificador de la marca.</param>
        /// <returns>ViewModel para la marca especifica.</returns>
        public async Task<MarcaViewModel> GetMarcaViewModel(int id)
        {
            MarcaViewModel vRet = new MarcaViewModel();
            var marca = await context.Marcas.FirstOrDefaultAsync(x => x.IdMarca == id);

            if (marca == null)
                return vRet;
            else
            {
                vRet = new MarcaViewModel
                {
                    IdMarca = marca.IdMarca!,
                    Descripcion = marca.Descripcion!,
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza una marca en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la marca.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarMarca(MarcaViewModel model)
        {
            Marca marca;

            if (model.IdMarca > 0)
            {
                marca = await context.Marcas.FirstOrDefaultAsync(x => x.IdMarca == model.IdMarca);

                if (marca == null)
                    throw new Exception("No se encontro la Marca");

            }
            else
            {
                marca = new Marca();
            }

            // Valida si la descripción de la marca esta duplicado en la misma empresa
            var marcaExistenteDescripcion = await context.Marcas.FirstOrDefaultAsync(x =>
            x.Descripcion!.ToLower() == model.Descripcion!.ToLower()
            && x.IdMarca != model.IdMarca);

            if (marcaExistenteDescripcion is not null)
                throw new Exception("Ya existe una Marca con el mismo nombre");

            marca.Descripcion = model.Descripcion;


            if (model.IdMarca > 0)
            {
                context.Marcas.Entry(marca);

                await context.SaveChangesAsync();
            }
            else
            {
                context.Marcas.Add(marca);
                await context.SaveChangesAsync();

            }
            return true;
        }

        /// <summary>
        /// Elimina una marca en la base de datos.
        /// </summary>
        /// <param name="IdMarca">Identificador de la marca.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarMarca(int IdMarca)
        {
            var marca = await context.Marcas.FirstOrDefaultAsync(x => x.IdMarca == IdMarca);

            if (marca is null)
            {
                throw new Exception("No se encontro la Marca");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.Marcas
              .Where(x => x.IdMarca == IdMarca)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }
    }
}