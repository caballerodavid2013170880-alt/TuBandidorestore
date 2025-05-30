using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        public async Task<List<Zona>> GetZona(int IdEmpresa)
        {

            var zona = await context.Zonas.Include(z => z.IdEmpresaNavigation).Where(z => z.IdEmpresa == IdEmpresa).ToListAsync();

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
                    Rfc = zona.Rfc!,
                    Domicilio = zona.Domicilio!,
                    Telefono1 = zona.Telefono1!,
                    Telefono2 = zona.Telefono2!,
                    Responsable = zona.Responsable!,
                    FechaApertura = zona.FechaApertura!,
                    Activo = zona.Activo!,
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
        public async Task<bool> AgregarZona(ZonaViewModel model, int IdEmpresa)
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

            // Valida si el nombre de la zona esta duplicado en la misma empresa
            var zonaExistenteNombre = await context.Zonas.FirstOrDefaultAsync(x =>
                x.NombreZona!.Trim().ToLower() == model.ZonaNombre!.Trim().ToLower() &&
                x.IdEmpresa == IdEmpresa &&
                x.IdZona != model.ZonaId);

            if (zonaExistenteNombre is not null)
                throw new Exception("Ya existe una Zona con el mismo nombre");


            // Valida si el RFC esta duplicado en la misma empresa
            var zonaExistenteRFC = await context.Zonas.FirstOrDefaultAsync(x =>
                x.Rfc!.Trim().ToLower() == model.Rfc!.Trim().ToLower() &&
                x.IdEmpresa == IdEmpresa &&
                x.IdZona != model.ZonaId);

            if (zonaExistenteRFC is not null)
                throw new Exception("Ya existe una Zona con el mismo RFC");


            // Valida si el RFC esta duplicado en otra empresa con diferente nombre
            var zonaRFCOtraEmpresa = await context.Zonas.FirstOrDefaultAsync(x =>
                x.Rfc!.Trim().ToLower() == model.Rfc!.Trim().ToLower() &&
                x.IdEmpresa != IdEmpresa &&
                x.NombreZona!.Trim().ToLower() != model.ZonaNombre!.Trim().ToLower());

            if (zonaRFCOtraEmpresa is not null)
                throw new Exception("Este RFC ya está registrado en otra empresa");

            zona.NombreZona = model.ZonaNombre;
            zona.Rfc = model.Rfc;
            zona.Domicilio = model.Domicilio;
            zona.Telefono1 = model.Telefono1;
            zona.Telefono2 = model.Telefono2;
            zona.Responsable = model.Responsable;
            zona.FechaApertura = model.FechaApertura;
            zona.IdEmpresa = IdEmpresa;
            zona.Activo = model.Activo;

            if (model.ZonaId > 0)
            {
                context.Zonas.Entry(zona);

                await context.SaveChangesAsync();
            }
            else
            {
                context.Zonas.Add(zona);
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
