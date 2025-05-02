using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Service.Seguridad;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Helper
{
  public class UsuarioHelper : IUsuarioHelper
  {
    const string claimTypeId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    const string claimTypePerfilId = "Perfil";
    const string claimTypeEmpresaId = "Empresa";
    private readonly IMenuService menuService;
    private readonly IPermisoService permisoService;
    private readonly IAdminService adminService;

    public UsuarioHelper(IMenuService menuService,
      IPermisoService permisoService,
      IAdminService adminService)
    {
      this.menuService = menuService;
      this.permisoService = permisoService;
      this.adminService = adminService;
    }

    /// <summary>
    /// obtiene las opciones del menu del usuario logueado
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<List<MenuViewModel>> GetOpcionesMenu(ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {

        var userPerfil = user.Claims
          .FirstOrDefault(i => i.Type == claimTypePerfilId)!.Value;

        var opcionesMenu = await menuService.GetOpcionesMenuByPefrilUsuario(int.Parse(userPerfil));

        return opcionesMenu;
      }

      return null!;
    }


    /// <summary>
    /// obtiene los permisos de la pagina para el usuario logueado agergar editar eliminar y ejecutar
    /// </summary>
    /// <param name="user"></param>
    /// <param name="opcion"></param>
    /// <returns></returns>
    public async Task<PermisoPaginaViewModel> GetPermisosPagina(ClaimsPrincipal user, EnumOpcionMenu opcion)
    {
      if (user.Identity!.IsAuthenticated)
      {

        var userPerfil = user.Claims
          .FirstOrDefault(i => i.Type == claimTypePerfilId)!.Value;


        var permisoPagina = await permisoService.GetPermisoByPerfilMenu(int.Parse(userPerfil), (int)opcion);

        return permisoPagina;
      }

      return null!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<List<AdminEmpresa>> GetEmpresas(ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {

        var userEmpresa = user.GetEmpresaId();
        var userIdentificador = user.GetUserId();

        var empresas = await adminService.GetEmpresaUsuario(userIdentificador);

        return empresas;
      }

      return null!;
    }

  }
}
