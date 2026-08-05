using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ICargaPeriodoService
    {
        //catalogos cascada
        Task <List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetRegions(int id_empresa);
        Task <List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetPlantasByRegion (int id_empresa, int id_region);
        Task <List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetZonasByPlanta (int id_empresa, int id_planta);
        Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetDepositosByZona (int id_empresa,int id_zona);


        //Metodos ccargas del periodo y estadisticas
        Task<List<CombCarga>> GetCargasPeriodo (int id_empresa, int id_deposito);
        Task<object> GetEstadisticasPeriodo (int id_empresa, int id_deposito);
    }
}
