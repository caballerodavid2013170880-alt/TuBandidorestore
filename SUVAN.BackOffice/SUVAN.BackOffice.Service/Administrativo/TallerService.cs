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
            //agregar
            if (id == 0)
            {
                return new TallerViewModel
                {
                    Regiones = await GetRegions(IdEmpresa)
                };
            }
            //editar
            var taller = await context.Tallers
                .Include(t => t.ZonaIdzonaNavigation)
                .Include(t => t.IdDepositoNavigation)
                .Where(t => t.IdTaller == id && t.IdDepositoNavigation.IdEmpresa == IdEmpresa)
                .FirstOrDefaultAsync();
            if (taller == null)
            {
                return new TallerViewModel();
            }

            //obtener jerarquia desde la zona

            var deposito = taller.IdDepositoNavigation;
            
            var idRegion = deposito.IdRegion;
            var idPlanta = deposito.IdPlanta;
            var idZona = deposito.IdZona;
            var idDeposito = deposito.IdDeposito;

            var model = new TallerViewModel
            {
                IdTaller = taller.IdTaller,

                NombreTaller = taller.NombreTaller,
                Domicilio = taller.Domicilio,
                Contacto = taller.Contacto,
                Telefono = taller.Telefono,
                Email = taller.Email,

                //jerarquia
                IdRegion = idRegion,
                IdPlanta = idPlanta,
                IdZona = idZona,
                IdDeposito = idDeposito,
            };

            //carga de catalogos
            model.Regiones = await GetRegions(IdEmpresa);
            model.Plantas = await GetPlantasByRegion(IdEmpresa, idRegion);
            model.Zonas = await GetZonasByPlanta(IdEmpresa, idPlanta);
            model.Depositos = await GetDepositosByZona(IdEmpresa, idZona);

            return model;
        }

        /// <summary>
        /// Agrega o actualiza un taller en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del taller.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarTaller(TallerViewModel model, int idEmpresa)
        {
            //Taller taller;
            //editar
            if (model.IdTaller > 0)
            {
                var taller = await context.Tallers
                    .Include(t => t.IdDepositoNavigation)
                    .FirstOrDefaultAsync(x => x.IdTaller == model.IdTaller && x.IdDepositoNavigation.IdEmpresa == idEmpresa);

                if (taller == null)
                {
                    throw new Exception("No se encontro el Taller");
                }

                //validar nombre duplicado
                var tallerExistente = await context.Tallers
                    .FirstOrDefaultAsync(x => x.NombreTaller!
                    .ToLower() == model.NombreTaller!
                    .ToLower() && x.IdTaller != model.IdTaller);

                if (tallerExistente is not null)
                {
                    throw new Exception("Ya existe un taller con el mismo nombre");
                }

                //solo campos editables
                taller.IdTaller = model.IdTaller;
                taller.NombreTaller = model.NombreTaller;
                taller.Domicilio = model.Domicilio;
                taller.Contacto = model.Contacto;
                taller.Telefono = model.Telefono;
                taller.Email = model.Email;
                //no se editan
                //taller.ZonaIdzona = model.IdZona;
                //taller.IdDeposito = model.IdDeposito;

                await context.SaveChangesAsync();
                return true;
            }

            //agregar
            // valida nombre duplicado
            var tallerExistenteNuevo = await context.Tallers.FirstOrDefaultAsync(x =>
            x.NombreTaller!.ToLower() == model.NombreTaller!.ToLower());

            if (tallerExistenteNuevo is not null)
            {
                throw new Exception("Ya existe un taller con el mismo nombre");
            }
            //validar que exista la zona
            var zona = await context.Zonas
                .FirstOrDefaultAsync(x => x.IdZona == model.IdZona && x.IdEmpresa == idEmpresa);

            if (zona == null)
            {
                throw new Exception($"La zona con ID {model.IdZona} no existe o no pertenece a la empresa");
            }

            //validar que exista el deposito
            var deposito = await context.Depositos
                .FirstOrDefaultAsync(x => x.IdDeposito == model.IdDeposito && x.IdEmpresa == idEmpresa && x.Activo.GetValueOrDefault() == 1);

            if (deposito == null)
            {
                throw new Exception($"El depósito con ID {model.IdDeposito} no existe, no pertenece a la empresa o no está activo");
            }

            //validar jerarquia
            if (deposito.IdZona != model.IdZona)
            {
                throw new Exception("El deposito no pertenece a la zona seleccionada.");
            }

            if (deposito.IdPlanta != model.IdPlanta)
            {
                throw new Exception("El deposito no pertenece a la planta seleccionada.");
            }

            if (deposito.IdRegion != model.IdRegion)
            {
                throw new Exception("El deposito no pertenece a la Region seleccionada");
            }

            //crear nuevo taller
            var nuevoTaller = new Taller
            {
                NombreTaller = model.NombreTaller,
                Domicilio = model.Domicilio,
                Contacto = model.Contacto,
                Telefono = model.Telefono,
                Email = model.Email,
                ZonaIdzona = model.IdZona,
                IdDeposito = model.IdDeposito
            };

            context.Tallers.Add(nuevoTaller);

            await context.SaveChangesAsync();

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
