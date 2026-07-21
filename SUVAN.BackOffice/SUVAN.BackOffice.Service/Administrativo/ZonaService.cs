using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Administrativo
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
            var zonas = await context.Zonas

            .Include(z => z.Id)                  // Región
                .Include(z => z.IdPlantaNavigation)  // Planta
                .Where(z => z.IdEmpresa == IdEmpresa)
                .OrderBy(z => z.Id.NombreRegion)
                .ThenBy(z => z.IdPlantaNavigation.NombrePlanta)
                .ThenBy(z => z.NombreZona)
                .ToListAsync();

            return zonas;
        }

        /// <summary>
        /// Obtiene el ViewModel de la zona específica.
        /// </summary>
        /// <param name="id">Identificador de la zona.</param>
        /// <returns>ViewModel para la zona especifica.</returns>
        public async Task<ZonaViewModel> GetZonaViewModel(int IdZona, int IdEmpresa)
        {
            var regiones = await context.Regions
                .Where(r => r.IdEmpresa == IdEmpresa && (r.Activo ?? 0) != 0)
                .OrderBy(r => r.NombreRegion)
                .Select(r => new ZonaViewModel.CatalogItemViewModel
                {
                    Id = r.IdRegion,
                    Nombre = r.NombreRegion
                }).ToListAsync();

            var vRet = new ZonaViewModel
            {
                Regiones = regiones,
                IdEmpresa = IdEmpresa,
                ActivoBool = true // Por defecto activo al crear
            };

            if (IdZona > 0)
            {
                var zona = await context.Zonas.FirstOrDefaultAsync(x => x.IdZona == IdZona && x.IdEmpresa == IdEmpresa);
                if (zona == null)
                    throw new Exception("La zona no pertenece a su empresa o no existe.");

                vRet.IdZona = zona.IdZona;
                vRet.ZonaNombre = zona.NombreZona;
                vRet.Rfc = zona.Rfc;
                vRet.Domicilio = zona.Domicilio;
                vRet.Telefono1 = zona.Telefono1;
                vRet.Telefono2 = zona.Telefono2;
                vRet.Responsable = zona.Responsable;
                vRet.FechaApertura = zona.FechaApertura;
                vRet.Activo = zona.Activo;
                vRet.IdRegion = zona.IdRegion;
                vRet.IdPlanta = zona.IdPlanta;

                vRet.Plantas = await context.Planta
                    .Where(p => p.IdRegion == zona.IdRegion && p.IdEmpresa == IdEmpresa)
                    .OrderBy(p => p.NombrePlanta)
                    .Select(p => new ZonaViewModel.CatalogItemViewModel
                    {
                        Id = p.IdPlanta,
                        Nombre = p.NombrePlanta
                    }).ToListAsync();
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

            // Validar seguridad de jerarquía
            bool regionValida = await context.Regions.AnyAsync(r => r.IdRegion == model.IdRegion && r.IdEmpresa == IdEmpresa);
            if (!regionValida) throw new Exception("La Región seleccionada no pertenece a su empresa.");

            bool plantaValida = await context.Planta.AnyAsync(p => p.IdPlanta == model.IdPlanta && p.IdRegion == model.IdRegion && p.IdEmpresa == IdEmpresa);
            if (!plantaValida) throw new Exception("La Planta seleccionada no pertenece a la Región o empresa indicada.");

            Zona zonas;
            if (model.IdZona > 0)
            {
                zona = await context.Zonas.FirstOrDefaultAsync(x => x.IdZona == model.IdZona && x.IdEmpresa == IdEmpresa);
                if (zona == null) throw new Exception("No se encontró la Zona o no tiene permisos.");
            }
            else
            {
                zona = new Zona();
            }

            // Validar nombre duplicado en la misma Planta
            bool zonaExistenteNombre = await context.Zonas.AnyAsync(x =>
                x.NombreZona!.Trim().ToLower() == model.ZonaNombre!.Trim().ToLower() &&
                x.IdPlanta == model.IdPlanta &&
                x.IdEmpresa == IdEmpresa &&
                x.IdZona != model.IdZona);
            if (zonaExistenteNombre) throw new Exception("Ya existe una Zona con el mismo nombre en esta Planta.");

            // Validar RFC a nivel Empresa
            bool rfcExistente = await context.Zonas.AnyAsync(x =>
                x.Rfc!.Trim().ToLower() == model.Rfc!.Trim().ToLower() &&
                x.IdEmpresa == IdEmpresa &&
                x.IdZona != model.IdZona);
            if (rfcExistente) throw new Exception("Ya existe una Zona con el mismo RFC en su empresa.");

            zona.NombreZona = model.ZonaNombre;
            zona.Rfc = model.Rfc;
            zona.Domicilio = model.Domicilio;
            zona.Telefono1 = model.Telefono1;
            zona.Telefono2 = model.Telefono2;
            zona.Responsable = model.Responsable;
            zona.FechaApertura = model.FechaApertura;
            zona.IdEmpresa = IdEmpresa;
            zona.Activo = model.Activo;
            zona.IdRegion = model.IdRegion;
            zona.IdPlanta = model.IdPlanta;

            if (model.IdZona > 0)
            {
                context.Zonas.Entry(zona).State = EntityState.Modified;
            }
            else
            {
                context.Zonas.Add(zona);
            }
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Elimina una zona en la base de datos.
        /// </summary>
        /// <param name="IdZona">Identificador de la zona.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        
        [HttpPost]
        public async Task<bool> EliminarZona(int idZona, int idEmpresa)
        {
            var zona = await context.Zonas.FirstOrDefaultAsync(x => x.IdZona == idZona && x.IdEmpresa == idEmpresa);
            if (zona is null) throw new Exception("No se encontró la Zona.");

            context.Zonas.Remove(zona);
            await context.SaveChangesAsync();
            return true;
        }   

    }
}
