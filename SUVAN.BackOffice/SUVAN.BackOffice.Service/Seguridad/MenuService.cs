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

            var permisosGroup = permisos.GroupBy(x => x.MenuIdmenuNavigation.MenuIdpadre);

            foreach (var permisoGroup in permisosGroup.OrderBy(pg => pg.Key)) // Ordenar grupos de menú principal
            {
                var menuId = permisoGroup.Key;
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

                    // Ordenar submenús alfabéticamente antes de procesarlos
                    var subMenuItems = permisoGroup
                        .Select(permiso => new MenuItemViewModel
                        {
                            Id = permiso.MenuIdmenuNavigation.Idmenu,
                            Titulo = permiso.MenuIdmenuNavigation.Titulo!,
                            Icono = permiso.MenuIdmenuNavigation.Icono!,
                            Ruta = permiso.MenuIdmenuNavigation.Ruta!,
                        })
                        .OrderBy(subMenu => subMenu.Titulo) // Ordenar submenús
                        .ToList();

                    foreach (var subMenuItem in subMenuItems)
                    {
                        // Ordenar opciones hijas dentro del submenú
                        var opcionesHijas = permisos
                            .Where(x => x.MenuIdmenuNavigation.MenuIdpadre == subMenuItem.Id)
                            .Select(x => new MenuItemViewModel
                            {
                                Id = x.MenuIdmenuNavigation.Idmenu,
                                Titulo = x.MenuIdmenuNavigation.Titulo!,
                                Icono = x.MenuIdmenuNavigation.Icono!,
                                Ruta = x.MenuIdmenuNavigation.Ruta!,
                            })
                            .OrderBy(opcion => opcion.Titulo) // Ordenar opciones hijas
                            .ToList();

                        if (opcionesHijas.Any())
                        {
                            subMenuItem.SubMenuItems = opcionesHijas;
                        }
                    }

                    menuItem.SubMenuItems = subMenuItems;

                    if (menu.Idmenu != 90)
                    {
                        menuItems.Add(menuItem);
                    }
                }
            }

            // Ordenar los menús principales antes de devolver la lista
            return menuItems.OrderBy(menu => menu.MenuItem.Titulo).ToList();
        }
    }
}