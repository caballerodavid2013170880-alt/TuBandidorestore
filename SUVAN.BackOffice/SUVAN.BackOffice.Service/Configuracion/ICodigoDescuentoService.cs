using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface ICodigoDescuentoService
  {
    Task<List<Codigodescuento>> GetCodigoDescuentos();
    Task<AgregarDescuentoViewModel> GetCodigoDescuentoViewModel(int id);
    Task<bool> GuardarCodigoDescuento(AgregarDescuentoViewModel model);
  }
}