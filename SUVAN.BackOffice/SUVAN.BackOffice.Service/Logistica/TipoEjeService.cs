using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class TipoEjeService : ITipoEjeService
    {
        private readonly SuvanDbContext context;

        public TipoEjeService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<TipoEje>> GetTipoEje()
        {

            var eje = await context.TipoEjes.ToListAsync();

            return eje!;
        }

        /// <summary>
        /// Obtiene el ViewModel del Tipo de Eje específico.
        /// </summary>
        /// <param name="idTipoEje">Identificador del tipo de eje.</param>
        /// <returns>ViewModel del tipo de eje especifico </returns>
        public async Task<TipoEjeViewModel> GetTipoEjeViewModel(int idTipoEje)
        {
            TipoEjeViewModel vRet = new TipoEjeViewModel();
            var eje = await context.TipoEjes.FirstOrDefaultAsync(x => x.IdTipoEje == idTipoEje);

            if (eje == null)
                return vRet;
            else
            {
                vRet = new TipoEjeViewModel
                {
                    IdTipoEje = eje.IdTipoEje!,
                    Descripcion = eje.Descripcion!,
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza un tipo de eje en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del tipo de eje.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarTipoEje(TipoEjeViewModel model)
        {
            TipoEje eje;

            if (model.IdTipoEje > 0)
            {
                eje = await context.TipoEjes.FirstOrDefaultAsync(x => x.IdTipoEje == model.IdTipoEje);

                if (eje == null)
                    throw new Exception("No se encontro el Tipo de Eje");

            }
            else
            {
                eje = new TipoEje();
            }

            // Valida si la descripción del eje esta duplicado
            var ejeExistenteDescripcion = await context.TipoEjes.FirstOrDefaultAsync(x =>
            x.Descripcion!.ToLower() == model.Descripcion!.ToLower()
            && x.IdTipoEje != model.IdTipoEje);

            if (ejeExistenteDescripcion is not null)
                throw new Exception("Ya existe un Tipo de Eje con la misma descripción");

            eje.Descripcion = model.Descripcion;


            if (model.IdTipoEje > 0)
            {
                context.TipoEjes.Entry(eje);

                await context.SaveChangesAsync();
            }
            else
            {
                context.TipoEjes.Add(eje);
                await context.SaveChangesAsync();

            }
            return true;
        }

        /// <summary>
        /// Elimina una Tipo de Eje en la base de datos.
        /// </summary>
        /// <param name="IdTipoEje">Identificador del Tipo de Eje.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarTipoEje(int IdTipoEje)
        {
            var eje = await context.TipoEjes.FirstOrDefaultAsync(x => x.IdTipoEje == IdTipoEje);

            if (eje is null)
            {
                throw new Exception("No se encontro el Tipo de Eje");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.TipoEjes
              .Where(x => x.IdTipoEje == IdTipoEje)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }
    }
}
