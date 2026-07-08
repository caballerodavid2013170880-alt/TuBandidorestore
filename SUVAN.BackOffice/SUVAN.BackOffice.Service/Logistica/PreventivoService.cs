using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class PreventivoService : IPreventivoService
    {
        private readonly SuvanDbContext context;

        public PreventivoService(SuvanDbContext context) { this.context = context; }

        public async Task<List<Preventivo>> GetPreventivos(int idEmpresa, int? idRegion = null, int? idPlanta = null, int? idZona = null, int? idDeposito = null)
        {
            var query = context.Preventivos
                .Include(p => p.Id)
                .Include(p => p.IdPlantaNavigation)
                .Include(p => p.IdZonaNavigation)
                .Include(p => p.IdDepositoNavigation)
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdModeloNavigation)
                .Where(p => p.Idempresa == idEmpresa);

            // Filtros preparados para RBAC Jerárquico
            if (idRegion.HasValue && idRegion > 0) query = query.Where(p => p.IdRegion == idRegion);
            if (idPlanta.HasValue && idPlanta > 0) query = query.Where(p => p.IdPlanta == idPlanta);
            if (idZona.HasValue && idZona > 0) query = query.Where(p => p.IdZona == idZona);
            if (idDeposito.HasValue && idDeposito > 0) query = query.Where(p => p.IdDeposito == idDeposito);

            return await query.OrderByDescending(p => p.Fecharegistro).ToListAsync();
        }

        public async Task<PreventivoViewModel> GetPreventivoViewModel(int idEmpresa, int idPreventivo)
        {
            var vRet = new PreventivoViewModel { Idempresa = idEmpresa };

            vRet.Regiones = await GetRegiones(idEmpresa);
            vRet.Marcas = await context.Marcas.Select(m => new PreventivoViewModel.CatalogItemViewModel { Id = m.IdMarca, Nombre = m.Descripcion }).ToListAsync();
            vRet.ManosObra = await context.ManoObras.Select(mo => new PreventivoViewModel.CatalogItemViewModel { Id = mo.IdManoObra, Nombre = mo.DescripcionManoobra }).ToListAsync();

            if (idPreventivo > 0)
            {
                var preventivo = await context.Preventivos.FirstOrDefaultAsync(p => p.Idpreventivo == idPreventivo && p.Idempresa == idEmpresa);
                if (preventivo == null) throw new Exception("El mantenimiento preventivo no existe.");

                vRet.Idpreventivo = preventivo.Idpreventivo;
                vRet.NombrePreventivo = preventivo.NombrePreventivo;
                vRet.ObservacionesPreventivo = preventivo.ObservacionesPreventivo;
                vRet.FechaPrev = preventivo.FechaPrev;
                vRet.IdRegion = preventivo.IdRegion ?? 0;
                vRet.IdPlanta = preventivo.IdPlanta ?? 0;
                vRet.IdZona = preventivo.IdZona ?? 0;
                vRet.IdDeposito = preventivo.IdDeposito ?? 0;
                vRet.IdMarca = preventivo.IdMarca;
                vRet.IdModelo = preventivo.IdModelo;

                if (vRet.IdRegion > 0) vRet.Plantas = await GetPlantasPorRegion(vRet.IdRegion);
                if (vRet.IdPlanta > 0) vRet.Zonas = await GetZonasPorPlanta(vRet.IdPlanta);
                if (vRet.IdZona > 0) vRet.Depositos = await GetDepositosPorZona(vRet.IdZona);
                if (vRet.IdMarca.HasValue) vRet.Modelos = await GetModelosPorMarca(vRet.IdMarca.Value);
            }
            return vRet;
        }

        public async Task<int> AgregarPreventivoAjax(PreventivoViewModel model, int idEmpresa, int idUsuario)
        {
            Preventivo preventivo;
            if (model.Idpreventivo > 0)
            {
                preventivo = await context.Preventivos.FirstOrDefaultAsync(p => p.Idpreventivo == model.Idpreventivo && p.Idempresa == idEmpresa);
                if (preventivo == null) throw new Exception("Registro no encontrado.");
            }
            else
            {
                preventivo = new Preventivo();
                preventivo.Idpreventivo = (await context.Preventivos.MaxAsync(p => (int?)p.Idpreventivo) ?? 0) + 1;
                context.Preventivos.Add(preventivo);
            }

            preventivo.Idempresa = idEmpresa;
            preventivo.NombrePreventivo = model.NombrePreventivo;
            preventivo.ObservacionesPreventivo = model.ObservacionesPreventivo;
            preventivo.FechaPrev = model.FechaPrev.Value;
            preventivo.IdRegion = model.IdRegion > 0 ? model.IdRegion : null;
            preventivo.IdPlanta = model.IdPlanta > 0 ? model.IdPlanta : null;
            preventivo.IdZona = model.IdZona > 0 ? model.IdZona : null;
            preventivo.IdDeposito = model.IdDeposito > 0 ? model.IdDeposito : null;
            preventivo.IdMarca = model.IdMarca > 0 ? model.IdMarca : null;
            preventivo.IdModelo = model.IdModelo;
            preventivo.Idusuario = idUsuario;
            preventivo.Fecharegistro = DateTime.Now;

            await context.SaveChangesAsync();
            return preventivo.Idpreventivo;
        }

        public async Task<List<PreventivoViewModel.CatalogItemViewModel>> GetRegiones(int idEmpresa) =>
            await context.Regions.Where(r => r.IdEmpresa == idEmpresa).Select(r => new PreventivoViewModel.CatalogItemViewModel { Id = r.IdRegion, Nombre = r.NombreRegion }).ToListAsync();

        public async Task<List<PreventivoViewModel.CatalogItemViewModel>> GetPlantasPorRegion(int idRegion) =>
            await context.Planta.Where(p => p.IdRegion == idRegion).Select(p => new PreventivoViewModel.CatalogItemViewModel { Id = p.IdPlanta, Nombre = p.NombrePlanta }).ToListAsync();

        public async Task<List<PreventivoViewModel.CatalogItemViewModel>> GetZonasPorPlanta(int idPlanta) =>
            await context.Zonas.Where(z => z.IdPlanta == idPlanta).Select(z => new PreventivoViewModel.CatalogItemViewModel { Id = z.IdZona, Nombre = z.NombreZona }).ToListAsync();

        public async Task<List<PreventivoViewModel.CatalogItemViewModel>> GetDepositosPorZona(int idZona) =>
            await context.Depositos.Where(d => d.IdZona == idZona).Select(d => new PreventivoViewModel.CatalogItemViewModel { Id = d.IdDeposito, Nombre = d.NombreDeposito }).ToListAsync();

        public async Task<List<PreventivoViewModel.CatalogItemViewModel>> GetModelosPorMarca(short idMarca) =>
            await context.Modelos.Where(m => m.IdMarca == idMarca).Select(m => new PreventivoViewModel.CatalogItemViewModel { Id = m.IdModelo, Nombre = m.Descripcion }).ToListAsync();

        public async Task<bool> GenerarDetallePreventivoAsync(int idPreventivo, int idManoObra, DateTime fechaPrev, int idEmpresa, int idUsuario)
        {
            await context.Database.ExecuteSqlRawAsync("CALL sp_GenerarDetallePreventivo({0}, {1}, {2}, {3})", idPreventivo, idManoObra, fechaPrev, idUsuario);
            return true;
        }

        public async Task<DetalleGeneralViewModel> GetDetalleGeneralAsync(int idEmpresa, int idPreventivo, int? idRegion = null, int? idPlanta = null)
        {
            var query = context.Preventivos
                .Include(x => x.IdPlantaNavigation)
                .Include(x => x.IdDepositoNavigation)
                .Include(x => x.IdMarcaNavigation)
                .Include(x => x.IdModeloNavigation)
                .Include(x => x.DetPrevs)
                .Where(x => x.Idempresa == idEmpresa && x.Idpreventivo == idPreventivo);

            if (idRegion.HasValue && idRegion > 0) query = query.Where(x => x.IdRegion == idRegion);
            if (idPlanta.HasValue && idPlanta > 0) query = query.Where(x => x.IdPlanta == idPlanta);

            var p = await query.FirstOrDefaultAsync();
            if (p == null) return null;

            var det = p.DetPrevs.FirstOrDefault();

            return new DetalleGeneralViewModel
            {
                IdPreventivo = p.Idpreventivo,
                NombrePreventivo = p.NombrePreventivo,
                Planta = p.IdPlantaNavigation?.NombrePlanta ?? "N/A",
                Deposito = p.IdDepositoNavigation?.NombreDeposito ?? "N/A",
                Marca = p.IdMarcaNavigation?.Descripcion ?? "N/A",
                Modelo = p.IdModeloNavigation?.Descripcion ?? "N/A",
                FechaPrev = p.FechaPrev,
                FechaRegistro = p.Fecharegistro,
                CostoUnitario = det?.CostoUnitario ?? 0,
                IvaUnitario = (det?.CostoUnitario ?? 0) * 0.16m,
                IvaTotal = det?.Iva ?? 0,
                CostoTotal = p.CostoTotal ?? det?.CostoTotal ?? 0
            };
        }

        public async Task<List<DetPrevMoItemViewModel>> GetDetalleVehiculosAsync(int idEmpresa, int idPreventivo = 0, int? idRegion = null)
        {
            var query = context.DetPrevMos
                .Include(d => d.IdpreventivoNavigation)
                .Include(d => d.IdManoObraNavigation)
                  .ThenInclude(mo => mo.ManoObraDetalles)
                .Include(d => d.IdVehiculoNavigation)
                .Where(d => d.IdpreventivoNavigation.Idempresa == idEmpresa);

            if (idPreventivo > 0) query = query.Where(d => d.Idpreventivo == idPreventivo);
            if (idRegion.HasValue && idRegion > 0) query = query.Where(d => d.IdpreventivoNavigation.IdRegion == idRegion);

            return await query.Select(d => new DetPrevMoItemViewModel
            {
                IdPrevMo = d.IdPrevMo,
                IdPreventivo = d.Idpreventivo,
                NombrePreventivo = d.IdpreventivoNavigation.NombrePreventivo,
                ManoObra = d.IdManoObraNavigation.DescripcionManoobra,
                Actividades = d.IdManoObraNavigation.ManoObraDetalles.Select(a => a.DescripcionActividad).ToList(),
                Placas = d.IdVehiculoNavigation.Placas,
                Vin = d.IdVehiculoNavigation.Vin,
                Iva = d.Iva,
                CostoTotalUnitario = d.CostoTotalUnitario,
                FechaPrev = d.IdpreventivoNavigation.FechaPrev
            }).ToListAsync();
        }

        public async Task<List<PreventivoViewModel.CatalogItemViewModel>> GetDropdownPreventivos(int idEmpresa, int? idRegion = null, int? idPlanta = null)
        {
            var query = context.Preventivos.Where(p => p.Idempresa == idEmpresa);
            if (idRegion.HasValue && idRegion > 0) query = query.Where(p => p.IdRegion == idRegion);
            if (idPlanta.HasValue && idPlanta > 0) query = query.Where(p => p.IdPlanta == idPlanta);

            return await query.Select(p => new PreventivoViewModel.CatalogItemViewModel { Id = p.Idpreventivo, Nombre = p.NombrePreventivo }).ToListAsync();
        }
    }
}