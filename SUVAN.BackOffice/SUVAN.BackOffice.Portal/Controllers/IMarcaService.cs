
using SUVAN.BackOffice.Models.ViewModel.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    internal interface IMarcaService
    {
        Task<bool> AgregarMarca(CausaMantenimientoViewModel.MarcaViewModel model);
        Task EliminarMarca(object idMarca);
        Task<string?> GetMarcas();
        Task<string?> GetMarcaViewModel(int id);
    }
}