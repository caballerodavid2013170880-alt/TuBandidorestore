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
    public class CargaPeriodoService : ICargaPeriodoService
    {
        private readonly SuvanDbContext context;

        public CargaPeriodoService(SuvanDbContext context)
        {
            this.context = context;
        }


        //Region combos en cascada
        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetRegions(int id_empresa)
        {
            return await context.Regions
                .Where(x => x.IdEmpresa == id_empresa)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel {
                    Id = x.IdRegion,
                    Nombre = x.NombreRegion
                }).ToListAsync();
        }

        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetPlantasByRegion(int id_empresa, int id_region)
        {
            return await context.Planta
                .Where(x => x.IdEmpresa == id_empresa && x.IdRegion == (short)id_region)
                .Select(x => new VehiculoDetalleViewModel.CatalogItemViewModel {
                    Id = x.IdPlanta,
                    Nombre = x.NombrePlanta
                }).ToListAsync();
        }

        public async Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetZonasByPlanta(int id_empresa, int id_planta)
        {
            return await context.Zonas
                .Where(x => x.IdEmpresa == id_empresa && x.IdPlanta == id_planta)
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



        //logica para cargas del periodo

        /// <summary>
        /// Obtiene las cargas del periodo para el deposito seleccionado incluyendo relaciones clave.
        /// <summary>
        public async Task<List<CombCarga>> GetCargasPeriodo(int id_empresa, int id_deposito)
        {
            return await context.CombCargas
                .Include(x => x.IdVehiculoNavigation)
                .Include(x => x.IdCombNavigation) //tipo de combustible para mostrar su descripcion 
                .Where(x => x.Idempresa == id_empresa && x.IdDeposito == id_deposito)
                .OrderByDescending(x => x.Fecha)
                .ToListAsync();
        }

        /// <summary>
        /// Calcula las estadisticas del periodo para probar los indicadores de totales, litros importe, etc.
        /// <summary>
        public async Task<object> GetEstadisticasPeriodo(int id_empresa, int id_deposito)
        {
            var cargas = await context.CombCargas
                .Where(x => x.Idempresa == id_empresa && x.IdDeposito == id_deposito)
                .ToListAsync();

            int cargasTotales = cargas.Count;
            double consumoLitros = cargas.Sum(x => x.Litros);
            double importeTotal = cargas.Sum(x => x.Importe);
            double kmsRecorridosTotales = cargas.Sum(x => x.KmRecorridos);

            //primedios seguros para evitar diision entre cero
            double kilometrosLitroPromedio = consumoLitros > 0 ? kmsRecorridosTotales / consumoLitros : 0;
            double costoXKmRecorrido = kmsRecorridosTotales > 0 ? importeTotal / kmsRecorridosTotales : 0;

            return new
            {
                CargasTotales = cargasTotales,
                ConsumoLitros = consumoLitros.ToString("N2"),
                ImporteTotal = importeTotal.ToString("N2"),
                KmsRecorridosTotales = kmsRecorridosTotales.ToString("N2"),
                KilometrosLitro = kilometrosLitroPromedio.ToString("N2"),
                CostoXKmRecorrido = costoXKmRecorrido.ToString("N2")
            };
        }
    }
}
