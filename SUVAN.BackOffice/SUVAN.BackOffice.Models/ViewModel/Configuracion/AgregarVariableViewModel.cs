using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  public class AgregarVariableViewModel
  {
    public List<VariableViewModel> Variables { get; set; } = new();
    public int CantidadVariables { get; set; } = 0;
  }

  public class VariableViewModel
  {
    public int VariableId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;

    public List<VariableListViewModel>? Lista { get; set; } = new();
  }

  public class VariableListViewModel
  {
    public string id { get; set; } = string.Empty;
    public string valor { get; set; } = string.Empty;
  }
}
