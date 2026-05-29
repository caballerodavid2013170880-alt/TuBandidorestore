using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ICausaSiniestroService
    {
        Task<List<CausaSiniestro>> GetCausaSiniestro();
        Task<CausaSiniestroViewModel> GetCausaSiniestroViewModel(int id);
        Task<bool> AgregarCausaSiniestro(CausaSiniestroViewModel model);
        Task<bool> EliminarCausaSiniestro(int IdCausaSiniestro);
    }
}