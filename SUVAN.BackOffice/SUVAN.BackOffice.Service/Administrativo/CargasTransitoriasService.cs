using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Logistica;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class CargasTransitoriasService : ICargasTransitoriasService
    {
        private readonly SuvanDbContext context;

        public CargasTransitoriasService(SuvanDbContext context)
        {
            this.context = context;
        }


        //Region combos en cascada
        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetRegions (int id_empresa)
        {
            return await context.Regions
                .Where(x => x.IdEmpresa == id_empresa)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel {
                    Id = x.IdRegion,
                    Nombre = x.NombreRegion
                }).ToListAsync();
        }

        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetPlantasByRegion (int id_empresa, int id_region)
        {
            return await context.Planta
                .Where(x => x.IdEmpresa == id_empresa && x.IdRegion == (short) id_region)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel {
                    Id = x.IdPlanta,
                    Nombre = x.NombrePlanta
                }).ToListAsync();
        }

        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetZonasByPlanta(int id_empresa, int id_planta)
        {
            return await context.Zonas
                .Where(x => x.IdEmpresa == id_empresa && x.IdRegion == id_planta)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel {
                    Id = x.IdZona,
                    Nombre = x.NombreZona
                }).ToListAsync();
        }


        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetDepositosByZona(int id_empresa, int id_zona)
        {
            return await context.Depositos
                .Where(x => x.IdEmpresa == id_empresa && x.IdZona == id_zona && x.Activo.GetValueOrDefault() == 1)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel {
                    Id = x.IdDeposito,
                    Nombre = x.NombreDeposito
                }).ToListAsync();
        }



        //CargasTransitorias logica de negocio
        
        /// <summary>
        /// Obtiene las cargas cuyo traspaso sea 0 para el deposito seleccionado.
        /// <summary>
        public async Task<List<CombCarga>> GetCargasTransitorias(int id_empresa, int id_deposito)
        {
            return await context.CombCargas
                .Include(x => x.IdVehiculoNavigation)
                .Where(x => x.Idempresa == id_empresa && x.IdDeposito == id_deposito && x.Traspasar == 0)
                .ToListAsync();
        }

        /// <summary>
        /// Buscar vehiculo por numero economico .
        /// <summary>
        public async Task<int?> GetIdVehiculoByEconomico (string numeroEconomico, int id_empresa)
        {
            var vehiculo = await context.Vehiculos
                 .FirstOrDefaultAsync(x => x.Numeroeconomico == numeroEconomico && x.EmpresaIdempresa == id_empresa && x.Activo == 1);
            return vehiculo?.IdVehiculo;
        }


        /// <summary>
        /// Guardado masivo o actualizacion de cargas modificadas .
        /// <summary>
        public async Task<bool> SaveCargasBD(List<CombCarga> cargas)
        {
            if (cargas == null || !cargas.Any()) return false;

            foreach (var carga in cargas)
            {
                if (carga.IdCarga > 0)
                {
                    //Si ya existe, actualizamos su estado o valores modificados
                    context.Entry(carga).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
                else
                {
                    //registro nuevo
                    context.CombCargas.Add(carga);
                    await context.SaveChangesAsync();
                } 
            }
            return true;

        }
    }
}
