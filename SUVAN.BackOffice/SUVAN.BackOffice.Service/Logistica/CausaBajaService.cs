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
    public class CausaBajaService : ICausaBajaService
    {
        private readonly SuvanDbContext context;

        public CausaBajaService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<CausaBaja>> GetCausaBaja()
        {

            var causa = await context.CausaBajas.Include(x => x.IdBajaNavigation).ToListAsync();

            return causa!;
        }

        /// <summary>
        /// Obtiene el ViewModel de la Causa Baja del Vehiculo específico.
        /// </summary>
        /// <param name="idCausa">Identificador de la Causa Baja del Vehiculo.</param>
        /// <returns>ViewModel de la Baja especifica.</returns>
        public async Task<CausaBajaViewModel> GetCausaBajaViewModel(int idCausa)
        {
            CausaBajaViewModel vRet = new CausaBajaViewModel();
            var causa = await context.CausaBajas.FirstOrDefaultAsync(x => x.IdCausaBaja == idCausa);

            if (causa == null)
                return vRet;
            else
            {
                vRet = new CausaBajaViewModel
                {
                    IdCausaBaja = causa.IdCausaBaja!,
                    Descripcion = causa.Descripcion!,
                    IdBaja = causa.IdBaja!,
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza una causa en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la causa.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarCausaBajaVehiculo(CausaBajaViewModel model)
        {
            CausaBaja causa;

            if (model.IdCausaBaja > 0)
            {
                causa = await context.CausaBajas.FirstOrDefaultAsync(x => x.IdCausaBaja == model.IdCausaBaja);

                if (causa == null)
                    throw new Exception("No se encontro la Causa");

            }
            else
            {
                causa = new CausaBaja();
            }

            // Valida si la descripción de la causa baja esta duplicado
            var causaExistenteDescripcion = await context.CausaBajas.FirstOrDefaultAsync(x =>
            x.Descripcion!.ToLower() == model.Descripcion!.ToLower()
            && x.IdCausaBaja != model.IdCausaBaja);

            if (causaExistenteDescripcion is not null)
                throw new Exception("Ya existe una Causa> con la misma descripción");

            causa.Descripcion = model.Descripcion;
            causa.IdBaja = model.IdBaja;


            if (model.IdCausaBaja > 0)
            {
                context.CausaBajas.Entry(causa);

                await context.SaveChangesAsync();
            }
            else
            {
                context.CausaBajas.Add(causa);
                await context.SaveChangesAsync();

            }
            return true;
        }

        /// <summary>
        /// Elimina una causa en la base de datos.
        /// </summary>
        /// <param name="IdCausa">Identificador de la causa.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarCausaBajaVehiculo(int IdCausa)
        {
            var causa = await context.CausaBajas.FirstOrDefaultAsync(x => x.IdCausaBaja == IdCausa);

            if (causa is null)
            {
                throw new Exception("No se encontro la Causa");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.CausaBajas
              .Where(x => x.IdCausaBaja == IdCausa)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }

        public List<BajaVehiViewModel> ObtenerBajaVehiculo()
        {
            var baja = context.BajaVehis
                .Select(t => new BajaVehiViewModel
                {
                    IdBaja = t.IdBaja,
                    Descripcion = t.Descripcion
                }).ToList();

            return baja;
        }
    }
}
