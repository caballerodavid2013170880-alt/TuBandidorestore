using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Helper
{
  public interface IUsuarioHelper
  {
    Task<List<AdminEmpresa>> GetEmpresas(ClaimsPrincipal user);
    Task<List<MenuViewModel>> GetOpcionesMenu(ClaimsPrincipal user);
    Task<PermisoPaginaViewModel> GetPermisosPagina(ClaimsPrincipal user, EnumOpcionMenu opcion);
  }
}