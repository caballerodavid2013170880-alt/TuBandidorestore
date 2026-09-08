using FacturacionPegaso;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.ZonaViewModel;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class TallerService : ITallerService
    {
        private readonly SuvanDbContext context;

        public TallerService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Taller>> GetTaller(int IdEmpresa)
        {
            var talleres = await context.Tallers
                .Where(e => e.IdDepositoNavigation.IdEmpresa == IdEmpresa )
                .Include(t => t.ZonaIdzonaNavigation)
                .Include(t => t.IdDepositoNavigation)
                .ToListAsync();

            return talleres;
        }
        
        /// <summary>
        /// Obtiene el ViewModel del taller específico.
        /// </summary>
        /// <param name="id">Identificador del taller.</param>
        /// <returns>ViewModel para el taller especifico.</returns>
        public async Task<TallerViewModel> GetTallerViewModel(int id, int IdEmpresa)
        {

            var taller = await context.Tallers
                .Where(x => x.IdTaller == id)
                .Select(d => new TallerViewModel
                {
                    IdTaller = d.IdTaller,
                    NombreTaller = d.NombreTaller!,
                    Domicilio = d.Domicilio,
                    Contacto = d.Contacto,
                    Telefono = d.Telefono,
                    Email = d.Email,
                    IdZona = d.ZonaIdzona,
                    IdDeposito = d.IdDeposito,
                })
                .FirstOrDefaultAsync();

            return taller ?? new TallerViewModel();
        }

        /// <summary>
        /// Agrega o actualiza un taller en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del taller.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarTaller(TallerViewModel model)
        {
            Taller taller;

            if (model.IdTaller > 0)
            {
                taller = await context.Tallers.FirstOrDefaultAsync(x => x.IdTaller == model.IdTaller);

                if (taller == null)
                    throw new Exception("No se encontro el Taller");

            }
            else
            {
                taller = new Taller();
            }

            // valida si un depósito existe con el mismo nombre
            var tallerExistente = await context.Tallers.FirstOrDefaultAsync(x =>
            x.NombreTaller!.ToLower() == model.NombreTaller!.ToLower()
            && x.IdTaller != model.IdTaller);

            if (tallerExistente is not null)
                throw new Exception("Ya existe un taller con el mismo nombre");

            taller.IdTaller = model.IdTaller;
            taller.NombreTaller = model.NombreTaller;
            taller.Domicilio = model.Domicilio;
            taller.Contacto = model.Contacto;
            taller.Telefono = model.Telefono;
            taller.Email = model.Email;
            taller.ZonaIdzona = model.IdZona;
            taller.IdDeposito = model.IdDeposito;

            if (model.IdTaller > 0)
            {
                context.Tallers.Update(taller);

                await context.SaveChangesAsync();
            }
            else
            {
                context.Tallers.Add(taller);

                await context.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Elimina un taller en la base de datos.
        /// </summary>
        /// <param name="TallerId">Identificador del taller.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarTaller(int TallerId)
        {
            var taller = await context.Tallers.FirstOrDefaultAsync(x => x.IdTaller == TallerId);

            if (taller is null)
            {
                throw new Exception("No se encontro el Taller");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            await context.Tallers
              .Where(x => x.IdTaller == TallerId)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }



        //metodos implementados para carga de cascada 
        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetRegions(int id_empresa)
        {
            return await context.Regions
                .Where(x => x.IdEmpresa == id_empresa)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel
                {
                    Id = x.IdRegion,
                    Nombre = x.NombreRegion
                }).ToListAsync();
        }

        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetPlantasByRegion(int id_empresa, int id_region)
        {
            return await context.Planta
                .Where(x => x.IdEmpresa == id_empresa && x.IdRegion == (short)id_region)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel
                {
                    Id = x.IdPlanta,
                    Nombre = x.NombrePlanta
                }).ToListAsync();
        }

        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetZonasByPlanta(int id_empresa, int id_planta)
        {
            return await context.Zonas
                .Where(x => x.IdEmpresa == id_empresa && x.IdPlanta == id_planta)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel
                {
                    Id = x.IdZona,
                    Nombre = x.NombreZona
                }).ToListAsync();
        }

        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetDepositosByZona(int id_empresa, int id_zona)
        {
            return await context.Depositos
                .Where(x => x.IdEmpresa == id_empresa && x.IdZona == id_zona && x.Activo.GetValueOrDefault() == 1)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel
                {
                    Id = x.IdDeposito,
                    Nombre = x.NombreDeposito
                }).ToListAsync();
        }
    }
}
