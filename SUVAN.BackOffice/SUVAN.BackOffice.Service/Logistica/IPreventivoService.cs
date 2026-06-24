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
        /// /// <param name="fechaPrev"> Fecha programada para realizar el mantenimiento preventivo.</param>
        Task<bool> GenerarDetallePreventivoAsync(int idPreventivo, int idManoObra, DateTime fechaPrev, int idEmpresa, int idUsuario);
        /// <summary>
        /// Obtiene el resumen general y el registro masivo asociado a un plan preventivo.
        /// </summary>
        Task<DetalleGeneralViewModel> GetDetalleGeneralAsync(int idEmpresa, int idPreventivo);

        /// <summary>
        /// Obtiene el detalle unitario desglosado por cada vehículo coincidente.
        /// </summary>
        Task<List<DetPrevMoItemViewModel>> GetDetalleVehiculosAsync(int idEmpresa, int idPreventivo);
    }
}