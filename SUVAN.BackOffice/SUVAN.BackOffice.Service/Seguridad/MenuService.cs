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

            foreach (var permisoGroup in permisosGroup)
            {
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

                            // Busca opciones hijas dentro del submenú
                            var opcionesHijas = permisos.Where(x => x.MenuIdmenuNavigation.MenuIdpadre == subMenu.Idmenu)
                                                        .Select(x => new MenuItemViewModel
                                                        {
                                                            Id = x.MenuIdmenuNavigation.Idmenu,
                                                            Titulo = x.MenuIdmenuNavigation.Titulo!,
                                                            Icono = x.MenuIdmenuNavigation.Icono!,
                                                            Ruta = x.MenuIdmenuNavigation.Ruta!,
                                                        })
                                                        .ToList();

                            if (opcionesHijas.Any())
                            {
                                subMenuItem.SubMenuItems = opcionesHijas;
                            }

                            subMenuItems.Add(subMenuItem);
                        }
                    }

                    menuItem.SubMenuItems = subMenuItems;
                    if (menu.Idmenu !=90)
                    { menuItems.Add(menuItem); }
                    
                }
            }

            return menuItems;
        }
    }
}