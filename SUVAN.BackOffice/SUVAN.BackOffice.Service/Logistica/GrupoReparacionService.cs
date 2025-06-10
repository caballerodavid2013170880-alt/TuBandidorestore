using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.MantenimientoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class GrupoReparacionService : IGrupoReparacionService
    {
        private readonly SuvanDbContext context;

        public GrupoReparacionService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<GrupoReparacion>> GetGrupoReparacion()
        {

            var grupo = await context.GrupoReparacions.ToListAsync();

            return grupo!;
        }

        /// <summary>
        /// Obtiene el ViewModel del grupo específico.
        /// </summary>
        /// <param name="id">Identificador del grupo</param>
        /// <returns>ViewModel para el grupo específico.</returns>
        public async Task<GrupoReparacionViewModel> GetGrupoReparacionViewModel(int id)
        {
            GrupoReparacionViewModel vRet = new GrupoReparacionViewModel();
            var grupo = await context.GrupoReparacions.FirstOrDefaultAsync(x => x.IdGrupo == id);

            if (grupo == null)
                return vRet;
            else
            {
                vRet = new GrupoReparacionViewModel
                {
                    IdGrupo = grupo.IdGrupo!,
                    Descripcion = grupo.Descripcion,
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza un grupo en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del grupo.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarGrupoReparacion(GrupoReparacionViewModel model)
        {
            GrupoReparacion grupo;

            if (model.IdGrupo > 0)
            {
                grupo = await context.GrupoReparacions.FirstOrDefaultAsync(x => x.IdGrupo == model.IdGrupo);

                if (grupo == null)
                    throw new Exception("No se encontro el Grupo");

            }
            else
            {
                grupo = new GrupoReparacion();
            }

            // Valida si la descripción del grupo esta duplicado
            var zonaExistenteNombre = await context.GrupoReparacions.FirstOrDefaultAsync(x =>
                x.Descripcion!.Trim().ToLower() == model.Descripcion!.Trim().ToLower() &&
                x.IdGrupo != model.IdGrupo);

            if (zonaExistenteNombre is not null)
                throw new Exception("Ya existe un Grupo con la mismo descripción");

            grupo.Descripcion = model.Descripcion;

            if (model.IdGrupo > 0)
            {
                context.GrupoReparacions.Entry(grupo);

                await context.SaveChangesAsync();
            }
            else
            {
                context.GrupoReparacions.Add(grupo);
                await context.SaveChangesAsync();

            }
            return true;
        }

        /// <summary>
        /// Elimina un grupo en la base de datos.
        /// </summary>
        /// <param name="IdGrupo">Identificador del grupo.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarGrupoReperacion(int IdGrupo)
        {
            var grupo = await context.GrupoReparacions.FirstOrDefaultAsync(x => x.IdGrupo == IdGrupo);

            if (grupo is null)
            {
                throw new Exception("No se encontro el Grupo");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.GrupoReparacions
              .Where(x => x.IdGrupo == IdGrupo)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }
    }
}
