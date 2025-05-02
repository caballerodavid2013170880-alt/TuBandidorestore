using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
  public class PermisoPaginaViewModel
  {
    public int MenuId { get; set; }
    public int PerfilId { get; set; }
    public bool Agregar { get; set; } = false;
    public bool Editar { get; set; } = false;
    public bool Eliminar { get; set; } = false;
    public bool Ejecutar { get; set; } = false;
  }
}
