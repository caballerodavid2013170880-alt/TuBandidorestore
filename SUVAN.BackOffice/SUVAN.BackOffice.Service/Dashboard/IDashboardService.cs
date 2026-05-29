using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Dashboard;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Dashboard
{
  public interface IDashboardService
  {
    Task<List<RutasViewModel>> GetRutasHorarios(int empresaId);
    Task<ViajeIngresosOcupacionDashboard> GetViajeOcupacionIngreso(ViajeIngresosOcupacionFiltro model, int empresaId);
  }
}