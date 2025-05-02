using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public interface IPermisoService
  {
    Task<int> AgregarPermisoByPerfil(List<Permiso> permisos);
    Task<int> DeletePermisoByPerfil(int perfilId);
    Task<PermisoPaginaViewModel> GetPermisoByPerfilMenu(int perfilId, int menuId);
    Task<List<Permiso>> GetPermisoByUsuario(int perfilId);
  }
}
