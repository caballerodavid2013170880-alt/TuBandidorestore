using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IPreventivoService
    {
        Task<List<Preventivo>> GetPreventivos(int idEmpresa);
        Task<PreventivoViewModel> GetPreventivoViewModel(int idEmpresa, int idPreventivo);
        Task<bool> AgregarPreventivo(PreventivoViewModel model, int idEmpresa, int idUsuario);
        Task<List<PreventivoViewModel.DepositoItemViewModel>> GetDepositosPorPlanta(int idEmpresa, int idPlanta);
        Task<List<PreventivoViewModel.ModeloItemViewModel>> GetModelosPorMarca(short idMarca);
    }
}