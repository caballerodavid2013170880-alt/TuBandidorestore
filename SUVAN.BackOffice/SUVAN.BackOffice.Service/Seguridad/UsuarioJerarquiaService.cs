using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
    public class UsuarioJerarquiaService : IUsuarioJerarquiaService
    {
        private readonly SuvanDbContext context;

        public UsuarioJerarquiaService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<UsuarioJerarquium?> GetJerarquiaUsuario(string tipoUsuario, int idUsuario, int idEmpresa)
        {
            return await context.UsuarioJerarquia
                .Include(u => u.IdRegionNavigation)
                .Include(u => u.IdPlantaNavigation)
                .Include(u => u.IdZonaNavigation)
                .Include(u => u.IdDepositoNavigation)
                .Include(u => u.IdDeptoNavigation)
                .FirstOrDefaultAsync(u => u.TipoUsuario == tipoUsuario
                                       && u.IdUsuario == idUsuario
                                       && u.IdEmpresa == idEmpresa
                                       && u.Activo == 1
                                       && u.EsPrincipal == 1);
        }

        public async Task<List<UsuarioJerarquium>> GetJerarquiasUsuario(string tipoUsuario, int idUsuario, int idEmpresa)
        {
            return await context.UsuarioJerarquia
                .Include(u => u.IdRegionNavigation)
                .Include(u => u.IdPlantaNavigation)
                .Include(u => u.IdZonaNavigation)
                .Include(u => u.IdDepositoNavigation)
                .Include(u => u.IdDeptoNavigation)
                .Where(u => u.TipoUsuario == tipoUsuario
                         && u.IdUsuario == idUsuario
                         && u.IdEmpresa == idEmpresa
                         && u.Activo == 1)
                .OrderByDescending(u => u.EsPrincipal)
                .ThenBy(u => u.IdUsuarioJerarquia)
                .ToListAsync();
        }

        public async Task<bool> GuardarJerarquiaUsuario(UsuarioJerarquium model)
        {
            var existente = await context.UsuarioJerarquia
                .FirstOrDefaultAsync(u => u.TipoUsuario == model.TipoUsuario
                                       && u.IdUsuario == model.IdUsuario
                                       && u.IdEmpresa == model.IdEmpresa);

            if (existente != null)
            {
                existente.IdRegion = model.IdRegion;
                existente.IdPlanta = model.IdPlanta;
                existente.IdZona = model.IdZona;
                existente.IdDeposito = model.IdDeposito;
                existente.IdDepto = model.IdDepto;
                existente.EsPrincipal = model.EsPrincipal;
                existente.Activo = model.Activo;
                context.UsuarioJerarquia.Update(existente);
            }
            else
            {
                model.FechaRegistro = DateTime.Now;
                context.UsuarioJerarquia.Add(model);
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GuardarJerarquiasUsuario(string tipoUsuario, int idUsuario, int idEmpresa, List<UsuarioJerarquium> jerarquias)
        {
            await context.UsuarioJerarquia
                .Where(u => u.TipoUsuario == tipoUsuario
                         && u.IdUsuario == idUsuario
                         && u.IdEmpresa == idEmpresa)
                .ExecuteDeleteAsync();

            if (jerarquias != null && jerarquias.Any())
            {
                foreach (var item in jerarquias)
                {
                    item.IdUsuarioJerarquia = 0;
                    item.TipoUsuario = tipoUsuario;
                    item.IdUsuario = idUsuario;
                    item.IdEmpresa = idEmpresa;
                    item.Activo = 1;
                    item.FechaRegistro = DateTime.Now;
                    context.UsuarioJerarquia.Add(item);
                }
                await context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<List<CatalogItemViewModel>> GetRegionesPorEmpresa(int idEmpresa)
        {
            return await context.Regions
                .Where(r => r.IdEmpresa == idEmpresa && r.Activo == 1)
                .OrderBy(r => r.NombreRegion)
                .Select(r => new CatalogItemViewModel
                {
                    Id = r.IdRegion,
                    Nombre = r.NombreRegion
                })
                .ToListAsync();
        }

        public async Task<List<CatalogItemViewModel>> GetPlantasPorRegion(int idEmpresa, int idRegion)
        {
            return await context.Planta
                .Where(p => p.IdEmpresa == idEmpresa && p.IdRegion == idRegion && p.Activo == 1)
                .OrderBy(p => p.NombrePlanta)
                .Select(p => new CatalogItemViewModel
                {
                    Id = p.IdPlanta,
                    Nombre = p.NombrePlanta
                })
                .ToListAsync();
        }

        public async Task<List<CatalogItemViewModel>> GetZonasPorPlanta(int idEmpresa, int idRegion, int idPlanta)
        {
            return await context.Zonas
                .Where(z => z.IdEmpresa == idEmpresa
                         && z.IdRegion == idRegion
                         && z.IdPlanta == idPlanta
                         && z.Activo == 1)
                .OrderBy(z => z.NombreZona)
                .Select(z => new CatalogItemViewModel
                {
                    Id = z.IdZona,
                    Nombre = z.NombreZona
                })
                .ToListAsync();
        }

        public async Task<List<CatalogItemViewModel>> GetDepositosPorZona(int idEmpresa, int idRegion, int idPlanta, int idZona)
        {
            return await context.Depositos
                .Where(d => d.IdEmpresa == idEmpresa
                         && d.IdRegion == idRegion
                         && d.IdPlanta == idPlanta
                         && d.IdZona == idZona
                         && d.Activo == 1)
                .OrderBy(d => d.NombreDeposito)
                .Select(d => new CatalogItemViewModel
                {
                    Id = d.IdDeposito,
                    Nombre = d.NombreDeposito
                })
                .ToListAsync();
        }

        public async Task<List<CatalogItemViewModel>> GetDeptosPorDeposito(int idEmpresa, int idRegion, int idPlanta, int idZona, int idDeposito)
        {
            return await context.Deptos
                .Where(d => d.IdEmpresa == idEmpresa
                         && d.IdRegion == idRegion
                         && d.IdPlanta == idPlanta
                         && d.IdZona == idZona
                         && d.IdDeposito == idDeposito
                         && d.Activo == 1)
                .OrderBy(d => d.NombreDepto)
                .Select(d => new CatalogItemViewModel
                {
                    Id = d.IdDepto,
                    Nombre = d.NombreDepto
                })
                .ToListAsync();
        }


        public async Task<bool> CambiarDepositoPrincipal(string tipoUsuario, int idUsuario, int idEmpresa, int idUsuarioJerarquia)
        {
            await context.UsuarioJerarquia
                .Where(u => u.TipoUsuario == tipoUsuario
                         && u.IdUsuario == idUsuario
                         && u.IdEmpresa == idEmpresa)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.EsPrincipal, (ulong)0));

            var jerarquia = await context.UsuarioJerarquia
                .FirstOrDefaultAsync(u => u.IdUsuarioJerarquia == idUsuarioJerarquia
                                       && u.TipoUsuario == tipoUsuario
                                       && u.IdUsuario == idUsuario
                                       && u.IdEmpresa == idEmpresa);

            if (jerarquia != null)
            {
                jerarquia.EsPrincipal = 1;
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
