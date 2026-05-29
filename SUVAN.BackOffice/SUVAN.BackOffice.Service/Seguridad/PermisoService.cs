using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public class PermisoService : IPermisoService
  {
    private readonly SuvanDbContext context;

    public PermisoService(SuvanDbContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// permisos asignados a un usuario
    /// </summary>
    /// <param name="perfilId"></param>
    /// <returns></returns>
    public async Task<List<Permiso>> GetPermisoByUsuario(int perfilId)
    {
      var permisos = await context.Permisos
        .Include(x => x.MenuIdmenuNavigation)
              .Where(x => x.PerfilIdperfil == perfilId
                           && x.Activo == 1).ToListAsync();

      return permisos!;
    }

    public async Task<PermisoPaginaViewModel> GetPermisoByPerfilMenu(int perfilId, int menuId)
    {
      var permisos = await context.Permisos
              .FirstOrDefaultAsync(x => x.PerfilIdperfil == perfilId
                                        && x.MenuIdmenu == menuId);

      if (permisos == null)
      {
        return null!;
      }
      var permisoPagina = new PermisoPaginaViewModel
      {
        MenuId = menuId,
        PerfilId = perfilId,
        Agregar = permisos?.Agregar == 1,
        Editar = permisos?.Modificar == 1,
        Eliminar = permisos?.Eliminar == 1,
        Ejecutar = permisos?.Ejecutar == 1,
      };

      return permisoPagina!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="perfilId"></param>
    /// <param name="permisos"></param>
    /// <returns></returns>
    public async Task<int> AgregarPermisoByPerfil(List<Permiso> permisos)
    {
      await context.Permisos
       .AddRangeAsync(permisos);
      return await context.SaveChangesAsync();
    }

    /// <summary>
    /// borrar los permisos de un perfil
    /// </summary>
    /// <param name="perfilId"></param>
    /// <returns></returns>
    public async Task<int> DeletePermisoByPerfil(int perfilId)
    {
      var noRows = await context.Permisos
        .Where(x => x.PerfilIdperfil == perfilId).ExecuteDeleteAsync();

      return noRows!;
    }
  }
}
