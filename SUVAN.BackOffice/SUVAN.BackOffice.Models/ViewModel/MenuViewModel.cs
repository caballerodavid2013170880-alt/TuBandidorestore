using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
  public class MenuViewModel
  {
    public MenuItemViewModel MenuItem { get; set; } = null!;
    public List<MenuItemViewModel> SubMenuItems { get; set; } = new List<MenuItemViewModel>();
  }

  public class MenuItemViewModel
  {
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty;

  }
}
