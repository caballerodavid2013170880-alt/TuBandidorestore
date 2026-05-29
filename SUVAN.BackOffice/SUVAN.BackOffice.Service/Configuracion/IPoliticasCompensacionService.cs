using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface IPoliticasCompensacionService
  {
    Task<List<Politicascompensacion>> GetPoliticasCompensacion(int empresaId);
    Task<List<Politicascompensacion>> GetPoliticasCompensacionEmpresa(int empresaId);
    Task<AgregarPoliticaCompensacionViewModel> GetPoliticasCompensacionesEmpresaViewModel(int id, int empresaId);
    Task<AgregarPoliticaCompensacionViewModel> GetPoliticasCompensacionesViewModel(int id, int empresaId);
    Task<bool> GuardarPoliticaCompensacion(AgregarPoliticaCompensacionViewModel model, int empresaId);
    Task<bool> GuardarPoliticaCompensacionEmpresa(AgregarPoliticaCompensacionViewModel model, int empresaId);
  }
}