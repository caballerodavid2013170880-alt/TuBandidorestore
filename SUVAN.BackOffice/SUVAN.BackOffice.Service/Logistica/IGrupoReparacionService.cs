using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.MantenimientoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IGrupoReparacionService
    {
        Task<List<GrupoReparacion>> GetGrupoReparacion();

        /// <summary>
        /// Obtiene el ViewModel del grupo específico.
        /// </summary>
        /// <param name="id">Identificador del grupo</param>
        /// <returns>ViewModel para el grupo específico.</returns>
        Task<GrupoReparacionViewModel> GetGrupoReparacionViewModel(int id);

        /// <summary>
        /// Agrega o actualiza un grupo en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del grupo.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarGrupoReparacion(GrupoReparacionViewModel model);

        /// <summary>
        /// Elimina un grupo en la base de datos.
        /// </summary>
        /// <param name="IdGrupo">Identificador del grupo.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarGrupoReperacion(int IdGrupo);
    }
}
