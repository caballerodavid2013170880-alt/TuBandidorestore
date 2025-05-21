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
    public class TipoServicioService : ITipoServicioService
    {
        private readonly SuvanDbContext context;

        public TipoServicioService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene el listado de los Tipos de Servicios desde la base de datos.
        /// </summary>
        /// <returns>Lista de Tipos de Servicios.</returns
        public async Task<List<TipoServicio>> GetTipoServicio()
        {
            var tipoServicio = await context.TipoServicios.ToListAsync();

            return tipoServicio!;
        }

        /// <summary>
        /// Obtiene el ViewModel del servicio específico.
        /// </summary>
        /// <param name="id">Identificador del servicio.</param>
        /// <returns>ViewModel para el servicio especifico.</returns>
        public async Task<CausaMantenimientoViewModel.TipoServicioViewModel> GetTipoServicioViewModel(int id)
        {
            CausaMantenimientoViewModel.TipoServicioViewModel vRet = new CausaMantenimientoViewModel.TipoServicioViewModel();
            var servicio = await context.TipoServicios.FirstOrDefaultAsync(x => x.IdTiposervicio == id);

            if (servicio == null)
                return vRet;
            else
            {
                vRet = new CausaMantenimientoViewModel.TipoServicioViewModel
                {
                    IdTiposervicio = servicio.IdTiposervicio!,
                    ServicioNombre = servicio.Nombre!,
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza un servicio en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del servicio.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarTipoServicio(CausaMantenimientoViewModel.TipoServicioViewModel model)
        {
            TipoServicio servicio;

            if (model.IdTiposervicio > 0)
            {
                servicio = await context.TipoServicios.FirstOrDefaultAsync(x => x.IdTiposervicio == model.IdTiposervicio);

                if (servicio == null)
                    throw new Exception("No se encontro el Servicio");
            }
            else
            {
                servicio = new TipoServicio();
            }

            // valida si un Servicio existe con el mismo nombre
            var servicioExistente = await context.TipoServicios.FirstOrDefaultAsync(x =>
            x.Nombre!.ToLower() == model.ServicioNombre!.ToLower()
            && x.IdTiposervicio != model.IdTiposervicio);

            if (servicioExistente is not null)
                throw new Exception("Ya existe un Servicio con el mismo nombre");

            servicio.Nombre = model.ServicioNombre;

            if (model.IdTiposervicio > 0)
            {
                context.TipoServicios.Entry(servicio);

                await context.SaveChangesAsync();
            }
            else
            {
                context.TipoServicios.Add(servicio);
                await context.SaveChangesAsync();

                await context.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Elimina una servicio en la base de datos.
        /// </summary>
        /// <param name="IdServicio">Identificador del servicio.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarTipoServicio(int IdServicio)
        {
            var servicio = await context.TipoServicios.FirstOrDefaultAsync(x => x.IdTiposervicio == IdServicio);

            if (servicio is null)
            {
                throw new Exception("No se encontro el Servicio");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.TipoServicios
              .Where(x => x.IdTiposervicio == IdServicio)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }
    }
}
