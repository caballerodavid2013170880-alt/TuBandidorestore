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
        private readonly IUsuarioJerarquiaService usuarioJerarquiaService;

        public UsuarioHelper(IMenuService menuService,
      IPermisoService permisoService,
      IAdminService adminService,
      IUsuarioJerarquiaService usuarioJerarquiaService)
    {
      this.menuService = menuService;
      this.permisoService = permisoService;
      this.adminService = adminService;
      this.usuarioJerarquiaService = usuarioJerarquiaService;
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

        /// <summary>
        /// Obtiene las empresas asignadas al usuario y sus almacenes/depósitos asignados en la jerarquía
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<UsuarioEmpresaMenuViewModel>> GetEmpresasConDepositos(ClaimsPrincipal user)
        {
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userIdentificador = user.GetUserId();
                var adminEmpresas = await adminService.GetEmpresaUsuario(userIdentificador);
                if (adminEmpresas == null || !adminEmpresas.Any())
                {
                    return new List<UsuarioEmpresaMenuViewModel>();
                }

                var result = new List<UsuarioEmpresaMenuViewModel>();

                foreach (var ae in adminEmpresas)
                {
                    var empresaVm = new UsuarioEmpresaMenuViewModel
                    {
                        EmpresaId = ae.EmpresaIdempresa,
                        EmpresaNombre = ae.EmpresaIdempresaNavigation?.Nombre ?? string.Empty,
                        EsPrincipal = ae.Principal == 1
                    };

                    var jerarquias = await usuarioJerarquiaService.GetJerarquiasUsuario("Admin", userIdentificador, ae.EmpresaIdempresa);
                    if (jerarquias != null && jerarquias.Any())
                    {
                        empresaVm.Depositos = jerarquias
                          .Where(j => j.IdDepositoNavigation != null || (j.IdDeposito.HasValue && j.IdDeposito.Value > 0))
                          .Select(j => new UsuarioDepositoMenuViewModel
                          {
                              IdUsuarioJerarquia = j.IdUsuarioJerarquia,
                              DepositoId = j.IdDeposito,
                              DepositoNombre = j.IdDepositoNavigation?.NombreDeposito ?? $"Depósito #{j.IdDeposito}",
                              ZonaNombre = j.IdZonaNavigation?.NombreZona ?? string.Empty,
                              PlantaNombre = j.IdPlantaNavigation?.NombrePlanta ?? string.Empty,
                              RegionNombre = j.IdRegionNavigation?.NombreRegion ?? string.Empty,
                              EsPrincipal = j.EsPrincipal == 1
                          })
                          .ToList();
                    }

                    result.Add(empresaVm);
                }

                return result;
            }

            return new List<UsuarioEmpresaMenuViewModel>();
        }


        }
}
