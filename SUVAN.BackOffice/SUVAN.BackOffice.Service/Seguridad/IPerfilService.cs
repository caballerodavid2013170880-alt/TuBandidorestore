using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public interface IPerfilService
  {
    Task<AgregarPerfilViewModel> GetPerfilViewModel(int perfiId);
    Task<List<Perfil>> GetPerfiles();
    Task<int> AgregarPerfil(AgregarPerfilViewModel model);
    Task<bool> EliminarPerfil(int perfilId);
  }
}
