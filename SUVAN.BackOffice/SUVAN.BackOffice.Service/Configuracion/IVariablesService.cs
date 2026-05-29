using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface IVariablesService
  {
    Task<bool> AgregarVariables(AgregarVariableViewModel model, int empresaId, EnumTipoVariable tipo, string usuario);
    Task<AgregarVariableViewModel> GetVariablesEmpresa(int empresaId);
    Task<AgregarVariableViewModel> GetVariablesGlobales();
  }
}