using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class CausaMantenimientoService : ICausaMantenimientoService
    {
        private readonly SuvanDbContext context;

        public CausaMantenimientoService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<CausaMantenimiento>> GetCausaMantenimiento()
        {

            var grupo = await context.CausaMantenimientos.ToListAsync();

            return grupo!;
        }

        /// <summary>
        /// Obtiene el ViewModel de la causa específica.
        /// </summary>
        /// <param name="id">Identificador de la causa</param>
        /// <returns>ViewModel para la causa específica.</returns>
        public async Task<CausaMantenimientoViewModel> GetCausaMantenimientoViewModel(int id)
        {
            CausaMantenimientoViewModel vRet = new CausaMantenimientoViewModel();
            var causa = await context.CausaMantenimientos.FirstOrDefaultAsync(x => x.IdCausamantenimiento == id);

            if (causa == null)
                return vRet;
            else
            {
                vRet = new CausaMantenimientoViewModel
                {
                    IdCausamantenimiento = causa.IdCausamantenimiento!,
                    Descripcion = causa.Descripcion,
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
        public async Task<bool> AgregarCausaMantenimiento(CausaMantenimientoViewModel model)
        {
            CausaMantenimiento causa;

            if (model.IdCausamantenimiento > 0)
            {
                causa = await context.CausaMantenimientos.FirstOrDefaultAsync(x => x.IdCausamantenimiento == model.IdCausamantenimiento);

                if (causa == null)
                    throw new Exception("No se encontro la Causa de Mantenimiento");

            }
            else
            {
                causa = new CausaMantenimiento();
            }

            // Valida si la descripción de la causa de mantenimiento esta duplicado
            var causaExistenteDescripcion = await context.CausaMantenimientos.FirstOrDefaultAsync(x =>
                x.Descripcion!.Trim().ToLower() == model.Descripcion!.Trim().ToLower() &&
                x.IdCausamantenimiento != model.IdCausamantenimiento);

            if (causaExistenteDescripcion is not null)
                throw new Exception("Ya existe un Causa de Mantenimiento con la misma descripción");

            causa.Descripcion = model.Descripcion;

            if (model.IdCausamantenimiento > 0)
            {
                context.CausaMantenimientos.Entry(causa);

                await context.SaveChangesAsync();
            }
            else
            {
                context.CausaMantenimientos.Add(causa);
                await context.SaveChangesAsync();

            }
            return true;
        }

        /// <summary>
        /// Elimina una causa de mantenimiento en la base de datos.
        /// </summary>
        /// <param name="IdCausa">Identificador de la causa de mantenimiento.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarCausaMantenimiento(int IdCausa)
        {
            var causa = await context.CausaMantenimientos.FirstOrDefaultAsync(x => x.IdCausamantenimiento == IdCausa);

            if (causa is null)
            {
                throw new Exception("No se encontro la Causa de Mantenimiento");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.CausaMantenimientos
              .Where(x => x.IdCausamantenimiento == IdCausa)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }
    }
}
