using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface ILlantaInspeccionService
    {
        Task<LlantaInspeccionViewModel> GetViewModel(int idEmpresa);

        Task<LlantaConfiguracionVehiculoViewModel> GetConfiguracionVehiculo(int idVehiculo, int idEmpresa);

        Task<bool> GuardarInspeccion(LlantaInspeccionGuardarViewModel model, int idEmpresa, int idUsuario);
    }
}
