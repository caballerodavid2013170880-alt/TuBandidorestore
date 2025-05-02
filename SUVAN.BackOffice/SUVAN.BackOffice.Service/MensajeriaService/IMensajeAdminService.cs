using SUVAN.BackOffice.Models.ViewModel;

namespace SUVAN.BackOffice.Service.MensajeriaService
{
  public interface IMensajeAdminService
  {
    Task<List<MensajesAdminViewModel>> GetMessage(int adminId);
    Task<bool> MarcarMensajesLeidos(int conversacionId);
  }
}