using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface IChoferUnidadService
  {
    Task<bool> AgregarChoferUnidad(ChoferUnidadAgregarViewModel choferUnidadAgregarViewModel);
    Task<ReasignarChoferViewMolde> BuscarAsignaciones(ConsultaReasingacionViewModel model, int empresaId);
    Task<ChoferUnidadAgregarViewModel> ConsultarChoferesUnidad(ChoferUnidadAgregarViewModel model, int empresaId);
    Task EliminarAsignacionChoferUnidad(ReasignarChoferViewModel model, int empresaId);
    Task<ChoferUnidadViewModel> GetChoferUnidadViewModel(int empresaId);
    Task<ReasignarChoferViewMolde> GetReasignarChoferUnidad(int empresaId);
    Task<ChoferUnidadViewModel> GetReporte(ChoferUnidadViewModel model, int empresaId);
    Task GuardarReasignacion(ReasignarChoferViewModel model, int empresaId);
  }
}