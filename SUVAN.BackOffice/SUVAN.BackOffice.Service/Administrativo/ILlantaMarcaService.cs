using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface ILlantaMarcaService
    {
        Task<List<LlantaMarcaViewModel>> GetMarcas();

        Task<LlantaMarcaViewModel> GetMarcaViewModel(uint idMarcaLlanta);

        Task<bool> CrearMarca(LlantaMarcaViewModel model, int idUsuario);

        Task<bool> ActualizarMarca(LlantaMarcaViewModel model, int idUsuario);

        Task<bool> EliminarMarca(uint idMarcaLlanta, int idUsuario);
    }
}
