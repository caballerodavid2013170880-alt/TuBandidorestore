using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public interface IMenuService
  {
    Task<List<MenuViewModel>> GetOpcionesMenuByPefrilUsuario(int perfilId);

  }
}
