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
    public class ZonaService : IZonaService
    {
        private readonly SuvanDbContext context;

        public ZonaService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Zona>> GetZona()
        {
            var zona = await context.Zonas.ToListAsync();

            return zona!;
        }

        /// <summary>
        /// Obtiene el ViewModel de la zona específica.
        /// </summary>
        /// <param name="id">Identificador de la zona.</param>
        /// <returns>ViewModel para la zona especifica.</returns>
        public async Task<ZonaViewModel> GetZonaViewModel(int id)
        {
            ZonaViewModel vRet = new ZonaViewModel();
            var zona = await context.Zonas.FirstOrDefaultAsync(x => x.IdZona == id);

            if (zona == null)
                return vRet;
            else
            {
                vRet = new ZonaViewModel
                {
                    ZonaId = zona.IdZona!,
                    ZonaNombre = zona.NombreZona!,
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza una zona en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la zona.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarZona(ZonaViewModel model)
        {
            Zona zona;

            if (model.ZonaId > 0)
            {
                zona = await context.Zonas.FirstOrDefaultAsync(x => x.IdZona == model.ZonaId);

                if (zona == null)
                    throw new Exception("No se encontro la Zona");

            }
            else
            {
                zona = new Zona();
            }

            // valida si un depósito existe con el mismo nombre
            var zonaExistente = await context.Zonas.FirstOrDefaultAsync(x =>
            x.NombreZona!.ToLower() == model.ZonaNombre!.ToLower()
            && x.IdZona != model.ZonaId);

            if (zonaExistente is not null)
                throw new Exception("Ya existe una Zona con el mismo nombre");

            zona.NombreZona = model.ZonaNombre;

            if (model.ZonaId > 0)
            {
                context.Zonas.Entry(zona);

                await context.SaveChangesAsync();
            }
            else
            {
                context.Zonas.Add(zona);
                await context.SaveChangesAsync();

                await context.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Elimina una zona en la base de datos.
        /// </summary>
        /// <param name="IdZona">Identificador de la zona.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarZona(int IdZona)
        {
            var zona = await context.Zonas.FirstOrDefaultAsync(x => x.IdZona == IdZona);

            if (zona is null)
            {
                throw new Exception("No se encontro la Zona");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.Zonas
              .Where(x => x.IdZona == IdZona)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }
    }
}
