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
                .Include(p => p.Id) // Región / Planta asumiendo navegación
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
                .Select(p => new PreventivoViewModel.PlantaItemViewModel
                {
                    IdPlanta = p.IdPlanta,
                    Nombre = p.NombrePlanta
                }).ToListAsync();

            var marcas = await context.Marcas
                .OrderBy(m => m.Descripcion)
                .Select(m => new PreventivoViewModel.MarcaItemViewModel
                {
                    IdMarca = m.IdMarca,
                    Nombre = m.Descripcion
                }).ToListAsync();

            // Ignorar iva y activo de mano de obra según especificaciones
            var manosObra = await context.ManoObras
                .OrderBy(mo => mo.DescripcionManoobra)
                .Select(mo => new PreventivoViewModel.ManoObraItemViewModel
                {
                    IdManoObra = mo.IdManoObra,
                    Descripcion = mo.DescripcionManoobra
                }).ToListAsync();

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

                if (preventivo == null)
                    throw new Exception("El mantenimiento preventivo no existe o no pertenece a su empresa.");

                vRet.Idpreventivo = preventivo.Idpreventivo;
                vRet.NombrePreventivo = preventivo.NombrePreventivo;
                vRet.ObservacionesPreventivo = preventivo.ObservacionesPreventivo;
                vRet.IdPlanta = preventivo.IdPlanta;
                vRet.IdDeposito = preventivo.IdDeposito;
                vRet.Meses = preventivo.Meses;
                vRet.IdMarca = preventivo.IdMarca;
                vRet.IdModelo = preventivo.IdModelo;

                vRet.Depositos = await GetDepositosPorPlanta(idEmpresa, preventivo.IdPlanta);
                if (preventivo.IdMarca.HasValue)
                {
                    vRet.Modelos = await GetModelosPorMarca(preventivo.IdMarca.Value);
                }
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
            preventivo.IdPlanta = model.IdPlanta;
            preventivo.IdDeposito = model.IdDeposito;
            preventivo.Meses = model.Meses;
            preventivo.IdMarca = model.IdMarca;
            preventivo.IdModelo = model.IdModelo;
            preventivo.Idempresa = idEmpresa;

            // Asignación de auditoría
            preventivo.Idusuario = idUsuario;
            preventivo.Fecharegistro = DateTime.Now;

            // Nota: model.IdManoObra se procesa aquí para interactuar con Detalles si es requerido posteriormente.

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
                }).ToListAsync();
        }

        public async Task<List<PreventivoViewModel.ModeloItemViewModel>> GetModelosPorMarca(short idMarca)
        {
            return await context.Modelos
                .Where(m => m.IdMarca == idMarca)
                .OrderBy(m => m.Descripcion)
                .Select(m => new PreventivoViewModel.ModeloItemViewModel
                {
                    IdModelo = m.IdModelo,
                    Nombre = m.Descripcion
                }).ToListAsync();
        }
    }
}