using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
    public interface IUsuarioJerarquiaService
    {
        Task<UsuarioJerarquium?> GetJerarquiaUsuario(string tipoUsuario, int idUsuario, int idEmpresa);
        Task<List<UsuarioJerarquium>> GetJerarquiasUsuario(string tipoUsuario, int idUsuario, int idEmpresa);
        Task<bool> GuardarJerarquiaUsuario(UsuarioJerarquium model);
        Task<bool> GuardarJerarquiasUsuario(string tipoUsuario, int idUsuario, int idEmpresa, List<UsuarioJerarquium> jerarquias);
        Task<List<CatalogItemViewModel>> GetRegionesPorEmpresa(int idEmpresa);
        Task<List<CatalogItemViewModel>> GetPlantasPorRegion(int idEmpresa, int idRegion);
        Task<List<CatalogItemViewModel>> GetZonasPorPlanta(int idEmpresa, int idRegion, int idPlanta);
        Task<List<CatalogItemViewModel>> GetDepositosPorZona(int idEmpresa, int idRegion, int idPlanta, int idZona);
        Task<List<CatalogItemViewModel>> GetDeptosPorDeposito(int idEmpresa, int idRegion, int idPlanta, int idZona, int idDeposito);
    }
}
