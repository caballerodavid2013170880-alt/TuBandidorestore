using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface ILlantaService
    {
        Task<List<LlantaViewModel>> GetLlantas(int idEmpresa);
    }
}
