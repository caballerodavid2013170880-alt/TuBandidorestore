using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface IPreventivoService
    {
       Task<List<Preventivo>> GetPreventivos(int idEmpresa, int? idRegion = null, int? idPlanta = null, int? idZona = null, int? idDeposito = null);
        Task<PreventivoViewModel> GetPreventivoViewModel(int idEmpresa, int idPreventivo);
        Task<int> AgregarPreventivoAjax(PreventivoViewModel model, int idEmpresa, int idUsuario);
        Task<bool> GenerarDetallePreventivoAsync(int idPreventivo, int idManoObra, DateTime fechaPrev, int idEmpresa, int idUsuario);

        Task<List<PreventivoViewModel.CatalogItemViewModel>> GetRegiones(int idEmpresa);
        Task<List<PreventivoViewModel.CatalogItemViewModel>> GetPlantasPorRegion(int idRegion);
        Task<List<PreventivoViewModel.CatalogItemViewModel>> GetZonasPorPlanta(int idPlanta);
        Task<List<PreventivoViewModel.CatalogItemViewModel>> GetDepositosPorZona(int idZona);
        Task<List<PreventivoViewModel.CatalogItemViewModel>> GetModelosPorMarca(short idMarca);

        Task<DetalleGeneralViewModel> GetDetalleGeneralAsync(int idEmpresa, int idPreventivo, int? idRegion = null, int? idPlanta = null);
        Task<List<DetPrevMoItemViewModel>> GetDetalleVehiculosAsync(int idEmpresa, int idPreventivo = 0, int? idRegion = null);
        Task<List<PreventivoViewModel.CatalogItemViewModel>> GetDropdownPreventivos(int idEmpresa, int? idRegion = null, int? idPlanta = null);
    }
}