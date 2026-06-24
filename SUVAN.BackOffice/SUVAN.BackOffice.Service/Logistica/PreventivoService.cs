using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class PreventivoService : IPreventivoService
    {
        private readonly SuvanDbContext context;

        public PreventivoService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Preventivo>> GetPreventivos(int idEmpresa)
        {
            return await context.Preventivos
                .Include(p => p.IdPlantaNavigation)
                .Include(p => p.IdDepositoNavigation)
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdModeloNavigation)
                .Where(p => p.Idempresa == idEmpresa)
                .OrderByDescending(p => p.Fecharegistro)
                .ToListAsync();
        }

        public async Task<PreventivoViewModel> GetPreventivoViewModel(int idEmpresa, int idPreventivo)
        {
            var plantas = await context.Planta
                .Where(p => p.IdEmpresa == idEmpresa)
                .OrderBy(p => p.NombrePlanta)
                .Select(p => new PreventivoViewModel.PlantaItemViewModel { IdPlanta = p.IdPlanta, Nombre = p.NombrePlanta })
                .ToListAsync();

            var marcas = await context.Marcas
                .OrderBy(m => m.Descripcion)
                .Select(m => new PreventivoViewModel.MarcaItemViewModel { IdMarca = m.IdMarca, Nombre = m.Descripcion })
                .ToListAsync();

            var manosObra = await context.ManoObras
                .OrderBy(mo => mo.DescripcionManoobra)
                .Select(mo => new PreventivoViewModel.ManoObraItemViewModel { IdManoObra = mo.IdManoObra, Descripcion = mo.DescripcionManoobra })
                .ToListAsync();

            var vRet = new PreventivoViewModel
            {
                Idempresa = idEmpresa,
                Plantas = plantas,
                Marcas = marcas,
                ManosObra = manosObra
            };

            if (idPreventivo > 0)
            {
                var preventivo = await context.Preventivos
                    .FirstOrDefaultAsync(p => p.Idpreventivo == idPreventivo && p.Idempresa == idEmpresa);

                if (preventivo == null) throw new Exception("El mantenimiento preventivo no existe o no pertenece a su empresa.");

                vRet.Idpreventivo = preventivo.Idpreventivo;
                vRet.NombrePreventivo = preventivo.NombrePreventivo;
                vRet.ObservacionesPreventivo = preventivo.ObservacionesPreventivo;
                vRet.FechaPrev = preventivo.FechaPrev;

                // Mapeo seguro de nullables a int
                vRet.IdPlanta = preventivo.IdPlanta ?? 0;
                vRet.IdDeposito = preventivo.IdDeposito ?? 0;
                vRet.IdMarca = preventivo.IdMarca;
                vRet.IdModelo = preventivo.IdModelo; // Modelo no es nullable en la BD según tu nuevo código

                if (vRet.IdPlanta > 0) vRet.Depositos = await GetDepositosPorPlanta(idEmpresa, vRet.IdPlanta);
                if (preventivo.IdMarca.HasValue) vRet.Modelos = await GetModelosPorMarca(preventivo.IdMarca.Value);
            }
            return vRet;
        }

        public async Task<bool> AgregarPreventivo(PreventivoViewModel model, int idEmpresa, int idUsuario)
        {
            Preventivo preventivo;

            if (model.Idpreventivo > 0)
            {
                preventivo = await context.Preventivos
                    .FirstOrDefaultAsync(p => p.Idpreventivo == model.Idpreventivo && p.Idempresa == idEmpresa);
                if (preventivo == null) throw new Exception("El registro no existe.");
            }
            else
            {
                preventivo = new Preventivo();
                var lastId = await context.Preventivos
                    .OrderByDescending(p => p.Idpreventivo)
                    .Select(p => (int?)p.Idpreventivo)
                    .FirstOrDefaultAsync();
                preventivo.Idpreventivo = (lastId ?? 0) + 1;
                context.Preventivos.Add(preventivo);
            }

            preventivo.NombrePreventivo = model.NombrePreventivo;
            preventivo.ObservacionesPreventivo = model.ObservacionesPreventivo;
            preventivo.IdModelo = model.IdModelo;
            preventivo.FechaPrev = model.FechaPrev.Value;

            // Asignación tolerante a los nuevos campos nulos
            preventivo.IdPlanta = model.IdPlanta > 0 ? model.IdPlanta : null;
            preventivo.IdDeposito = model.IdDeposito > 0 ? model.IdDeposito : null;
            preventivo.IdMarca = model.IdMarca > 0 ? model.IdMarca : null;
            preventivo.Idempresa = idEmpresa;

            // Auditoría/COntrol
            preventivo.Idusuario = idUsuario;
            preventivo.Fecharegistro = DateTime.Now;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PreventivoViewModel.DepositoItemViewModel>> GetDepositosPorPlanta(int idEmpresa, int idPlanta)
        {
            return await context.Depositos
                .Where(d => d.IdEmpresa == idEmpresa && d.IdPlanta == idPlanta)
                .OrderBy(d => d.NombreDeposito)
                .Select(d => new PreventivoViewModel.DepositoItemViewModel 
                { 
                    IdDeposito = d.IdDeposito, 
                    Nombre = d.NombreDeposito 
                })
                .ToListAsync();
        }

        public async Task<List<PreventivoViewModel.ModeloItemViewModel>> GetModelosPorMarca(short idMarca)
        {
            return await context.Modelos
                .Where(m => m.IdMarca == idMarca)
                .OrderBy(m => m.Descripcion)
                .Select(m => new PreventivoViewModel.ModeloItemViewModel 
                { IdModelo = m.IdModelo, 
                    Nombre = m.Descripcion 
                })
                .ToListAsync();
        }

        /// ========================= <SP/> ====================================
        public async Task<bool> GenerarDetallePreventivoAsync(int idPreventivo, int idManoObra, DateTime fechaPrev, int idEmpresa, int idUsuario)
        {
            // 1. Validar existencia y permisos
            var preventivo = await context.Preventivos
                .FirstOrDefaultAsync(p => p.Idpreventivo == idPreventivo && p.Idempresa == idEmpresa);

            if (preventivo == null)
                throw new Exception("El plan de mantenimiento preventivo no existe o no tiene acceso.");

            var manoObra = await context.ManoObras.FirstOrDefaultAsync(m => m.IdManoObra == idManoObra);
            if (manoObra == null)
                throw new Exception("La Mano de Obra seleccionada no es válida o no existe.");

            await context.Database.ExecuteSqlRawAsync(
            "CALL sp_GenerarDetallePreventivo({0}, {1}, {2}, {3})",
            idPreventivo, idManoObra, fechaPrev, idUsuario // Pasamos la fecha al Stored Procedure
            );
            return true;
        }

        public async Task<DetalleGeneralViewModel> GetDetalleGeneralAsync(int idEmpresa, int idPreventivo)
        {
            var preventivo = await context.Preventivos
               // .Include(p => p.IdPlantaNavigation)  //
               // .Include(p => p.IdDepositoNavigation) //
                .Include(p => p.IdMarcaNavigation)
                .Include(p => p.IdModeloNavigation)
                .Include(p => p.DetPrevs)
                .FirstOrDefaultAsync(p => p.Idpreventivo == idPreventivo && p.Idempresa == idEmpresa);

            if (preventivo == null)
                throw new Exception("El plan de mantenimiento preventivo no existe o no tiene acceso.");

            var detPrevResumen = preventivo.DetPrevs.FirstOrDefault();

            return new DetalleGeneralViewModel
            {
                IdPreventivo = preventivo.Idpreventivo,
                NombrePreventivo = preventivo.NombrePreventivo,
                Marca = preventivo.IdMarcaNavigation?.Descripcion ?? "N/A",
                Modelo = preventivo.IdModeloNavigation?.Descripcion ?? "N/A",
                FechaPrev = preventivo.FechaPrev,
                CostoUnitario = detPrevResumen?.CostoUnitario ?? 0,
                IvaTotal = detPrevResumen?.Iva ?? 0,
                CostoTotal = preventivo.CostoTotal ?? detPrevResumen?.CostoTotal ?? 0,
                Detalles = preventivo.DetPrevs.Select(d => new DetPrevItemViewModel
                {
                    IdPrevDet = d.IdPrevDet,
                    CostoUnitario = d.CostoUnitario,
                    Iva = d.Iva,
                    CostoTotal = d.CostoTotal,
                    FechaRegistro = d.Fecharegistro
                }).ToList()
            };
        }

        public async Task<List<DetPrevMoItemViewModel>> GetDetalleVehiculosAsync(int idEmpresa, int idPreventivo)
        {
            return await context.DetPrevMos
                .Include(d => d.IdpreventivoNavigation)
                .Include(d => d.IdManoObraNavigation)
                .Where(d => d.Idpreventivo == idPreventivo && d.IdpreventivoNavigation.Idempresa == idEmpresa)
                .Select(d => new DetPrevMoItemViewModel
                {
                    IdPrevMo = d.IdPrevMo,
                    NombrePreventivo = d.IdpreventivoNavigation.NombrePreventivo,
                    ManoObra = d.IdManoObraNavigation.DescripcionManoobra,
                    Iva = d.Iva,
                    CostoTotalUnitario = d.CostoTotalUnitario,
                    FechaPrev = d.IdpreventivoNavigation.FechaPrev
                })
                .ToListAsync();
        }


    }
}