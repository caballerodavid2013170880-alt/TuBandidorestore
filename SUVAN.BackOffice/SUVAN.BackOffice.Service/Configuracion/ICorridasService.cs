using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface ICorridasService
  {
    Task<CorridaViewModel> AddNuevaCorrida();
    Task<bool> AgregarCorrida(AgregarCorridaViewModel model, int empresaId);
    Task<bool> AgregarDetalleRuta(DetalleRutaViewModel model, int empresaId);
    Task<bool> EliminaCorrida(int rutaId, int empresaId);
    Task<bool> EliminarHorarioCorrida(int corridaId, int empresaId);
    Task<bool> EliminaTodaCorrida(int rutaId, int empresaId);
    Task<List<Rutum>> GetCorridas(int empresaId);
    Task<AgregarCorridaViewModel> GetCorridasViewModel(int id, int empresaId);
    Task<DetalleRutaViewModel> GetDetalleRutaViewModel(int rutaId, int empresaId);
  }
}