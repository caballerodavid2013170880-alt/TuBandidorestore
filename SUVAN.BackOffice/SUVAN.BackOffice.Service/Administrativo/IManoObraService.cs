using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface IManoObraService
    {
        /// <summary>
        /// Obtiene el listado general de Mano de Obra.
        /// </summary>
        Task<List<ManoObraViewModel>> GetManoObras();
        /// <summary>
        /// Obtiene un registro de Mano de Obra con todos sus detalles.
        /// </summary>
        Task<ManoObraViewModel> GetManoObra(int idManoObra);
        /// <summary>
        /// Guarda (crea o actualiza) un registro de Mano de Obra y sus detalles.
        /// </summary>
        Task<int> GuardarManoObraAsync(ManoObraViewModel model, int idUsuario);
        /// <summary>
        /// Obtiene el consolidado de todas las actividades registradas en el sistema.
        /// </summary>
        Task<List<ManoObraDetalleViewModel>> GetTodasActividades();
        /// <summary>
        /// Obtiene los detalles específicos asociados a un servicio de Mano de Obra.
        /// </summary>
        Task<List<ManoObraDetalleViewModel>> GetActividadesPorManoObra(int idManoObra);
    }
}
