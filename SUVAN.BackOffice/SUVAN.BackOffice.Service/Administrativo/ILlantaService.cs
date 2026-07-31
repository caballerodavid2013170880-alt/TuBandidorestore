using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface ILlantaService
    {
        Task<List<LlantaViewModel>> GetLlantas(int idEmpresa);
        Task<LlantaCrearViewModel> GetCrearViewModel(int idEmpresa, string nombreEmpresa, LlantaCrearViewModel? model = null);
        Task<LlantaCrearViewModel?> GetEditarViewModel(ulong idLlanta, int idEmpresa, string nombreEmpresa);
        Task<bool> CrearLlanta(LlantaCrearViewModel model, int idEmpresa, int idUsuario);
        Task<bool> ActualizarLlanta(LlantaCrearViewModel model, int idEmpresa, int idUsuario);
        Task<bool> EliminarLlanta(ulong idLlanta, int idEmpresa, int idUsuario);
        Task<List<LlantaCrearViewModel.CatalogItemViewModel>> GetModelosPorMarca(int idMarcaLlanta);
        Task<LlantaCrearViewModel.ModeloDetalleViewModel?> GetDetalleModelo(int idModeloLlanta);
    }
}
