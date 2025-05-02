using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public class MenuService : IMenuService
  {
    private readonly SuvanDbContext context;
    private readonly IPermisoService permisoService;

    public MenuService(SuvanDbContext context, IPermisoService permisoService)
    {
      this.context = context;
      this.permisoService = permisoService;
    }


    /// <summary>
    /// genera opciones del menu por perfil de usuario
    /// </summary>
    /// <param name="perfilId"></param>
    /// <returns></returns>
    public async Task<List<MenuViewModel>> GetOpcionesMenuByPefrilUsuario(int perfilId)
    {
      List<MenuViewModel> menuItems = new List<MenuViewModel>();
      var permisos = await permisoService.GetPermisoByUsuario(perfilId);

      // agrupa las opciones de menu seccion
      var permisosGroup = permisos.GroupBy(x => x.MenuIdmenuNavigation.MenuIdpadre);

      foreach (var permisoGroup in permisosGroup)
      {
        // genera el menu e la seccion
        var menuId = permisoGroup.Key!;
        var menu = await context.Menus.FirstOrDefaultAsync(x => x.Idmenu == menuId);

        if (menu != null)
        {
          var menuItem = new MenuViewModel
          {
            MenuItem = new MenuItemViewModel
            {
              Id = menu.Idmenu,
              Titulo = menu.Titulo!,
              Icono = menu.Icono!,
              Ruta = menu.Ruta!,
            }
          };

          // genera las opciones del submenu
          List<MenuItemViewModel> subMenuItems = new List<MenuItemViewModel>();
          foreach (var permiso in permisoGroup)
          {
            var subMenu = permiso.MenuIdmenuNavigation;

            if (subMenu != null)
            {
              var subMenuItem = new MenuItemViewModel
              {
                Id = subMenu.Idmenu,
                Titulo = subMenu.Titulo!,
                Icono = subMenu.Icono!,
                Ruta = subMenu.Ruta!,
              };

              subMenuItems.Add(subMenuItem);
            }

            menuItem.SubMenuItems = subMenuItems;

          }

          menuItems.Add(menuItem);
        }
      }

      return menuItems;
    }
  }
}
