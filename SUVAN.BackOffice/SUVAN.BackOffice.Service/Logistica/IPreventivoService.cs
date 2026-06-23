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
        /// <summary>
        /// Ejecuta el procedimiento almacenado para generar el detalle del mantenimiento preventivo.
        /// </summary>
        /// <param name="idPreventivo">ID del plan preventivo maestro.</param>
        /// <param name="idManoObra">ID de la mano de obra seleccionada en la vista.</param>
        /// <param name="idEmpresa">ID de la empresa de la sesión.</param>
        /// <param name="idUsuario">ID del usuario que detona la acción.</param>
        Task<bool> GenerarDetallePreventivoAsync(int idPreventivo, int idManoObra, int idEmpresa, int idUsuario);
    }
}