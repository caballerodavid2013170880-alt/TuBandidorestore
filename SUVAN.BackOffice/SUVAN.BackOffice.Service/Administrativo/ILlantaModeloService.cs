using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface ILlantaModeloService
    {
        Task<List<LlantaModeloViewModel>> GetModelos();

        Task<LlantaModeloViewModel> GetModeloViewModel(uint idModeloLlanta);

        Task<LlantaModeloViewModel> GetCrearViewModel(LlantaModeloViewModel? model = null);

        Task<bool> CrearModelo(LlantaModeloViewModel model, int idUsuario);

        Task<bool> ActualizarModelo(LlantaModeloViewModel model, int idUsuario);

        Task<bool> EliminarModelo(uint idModeloLlanta, int idUsuario);
    }
}
