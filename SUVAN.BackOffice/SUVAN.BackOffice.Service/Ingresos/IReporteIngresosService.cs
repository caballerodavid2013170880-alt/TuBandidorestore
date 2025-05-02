using SUVAN.BackOffice.Models.ViewModel.Ingresos;

namespace SUVAN.BackOffice.Service.Ingresos
{
  public interface IReporteIngresosService
  {
    Task<ReporteIngresosViewModel> InitReporteIngresosGlobal();
    Task<ReporteIngresosViewModel> ReporteIngresosEmpresaSearch(ReporteIngresosViewModel model);
    Task<ReporteIngresosViewModel> ReporteIngresosSearch(ReporteIngresosViewModel model);
  }
}