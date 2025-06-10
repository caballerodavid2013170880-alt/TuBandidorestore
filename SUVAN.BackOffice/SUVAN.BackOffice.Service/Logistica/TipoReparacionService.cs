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
    public class TipoReparacionService : ITipoReparacionService
    {
        private readonly SuvanDbContext context;

        public TipoReparacionService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<TipoReparacion>> GetTipoReparacion()
        {
            var rerparacion = await context.TipoReparacions.ToListAsync();

            return rerparacion!;
        }

        /// <summary>
        /// Obtiene el ViewModel de la Reparación especifica.
        /// </summary>
        /// <param name="id">Identificador de la Reparacion.</param>
        /// <returns>ViewModel para la Reparación.</returns>
        public async Task<MantenimientoDetalleViewModel.TipoReparacionViewModel> GetTipoReparacionViewModel(int id)
        {
            MantenimientoDetalleViewModel.TipoReparacionViewModel vRet = new MantenimientoDetalleViewModel.TipoReparacionViewModel();
            var reparacion = await context.TipoReparacions.FirstOrDefaultAsync(x => x.IdTipoReparacion == id);

            if (reparacion == null)
                return vRet;
            else
            {
                vRet = new MantenimientoDetalleViewModel.TipoReparacionViewModel
                {
                    IdTipoReparacion = reparacion.IdTipoReparacion!,
                    Descripcion = reparacion.Descripcion!,
                    Grupo = reparacion.IdGrupo!,
                    Valor = reparacion.Valor!,
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza un Tipo de Reparación en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de l Reparación.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarTipoReparacion(MantenimientoDetalleViewModel.TipoReparacionViewModel model)
        {
            TipoReparacion reparacion;

            if (model.IdTipoReparacion > 0)
            {
                reparacion = await context.TipoReparacions.FirstOrDefaultAsync(x => x.IdTipoReparacion == model.IdTipoReparacion);

                if (reparacion == null)
                    throw new Exception("No se encontro el Tipo de Reparación");

            }
            else
            {
                reparacion = new TipoReparacion();
            }

            // valida si una reparación existe con las misma descripción
            var reparacionExistente = await context.TipoReparacions.FirstOrDefaultAsync(x =>
            x.Descripcion!.ToLower() == model.Descripcion!.ToLower()
            && x.IdTipoReparacion != model.IdTipoReparacion);

            if (reparacionExistente is not null)
                throw new Exception("Ya existe una Reparación con la misma descripción");

            reparacion.Descripcion = model.Descripcion;
            reparacion.IdGrupo = model.Grupo;
            reparacion.Valor = model.Valor;

            if (model.IdTipoReparacion > 0)
            {
                context.TipoReparacions.Entry(reparacion);

                await context.SaveChangesAsync();
            }
            else
            {
                context.TipoReparacions.Add(reparacion);
                await context.SaveChangesAsync();

            }
            return true;
        }

        /// <summary>
        /// Elimina una reparación en la base de datos.
        /// </summary>
        /// <param name="IdReparacion">Identificador de la Reparación.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarTipoReparacion(int IdReparacion)
        {
            var reparacion = await context.TipoReparacions.FirstOrDefaultAsync(x => x.IdTipoReparacion == IdReparacion);

            if (reparacion is null)
            {
                throw new Exception("No se encontro la Reparación");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.TipoReparacions
              .Where(x => x.IdTipoReparacion == IdReparacion)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }

    }
}
