using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.MantenimientoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ITipoReparacionService
    {
        Task<List<TipoReparacion>> GetTipoReparacion();

        /// <summary>
        /// Obtiene el ViewModel de la Reparación especifica.
        /// </summary>
        /// <param name="id">Identificador de la Reparacion.</param>
        /// <returns>ViewModel para la Reparación.</returns>
        Task<MantenimientoDetalleViewModel.TipoReparacionViewModel> GetTipoReparacionViewModel(int id);

        /// <summary>
        /// Agrega o actualiza un Tipo de Reparación en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de l Reparación.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarTipoReparacion(MantenimientoDetalleViewModel.TipoReparacionViewModel model);

        /// <summary>
        /// Elimina una reparación en la base de datos.
        /// </summary>
        /// <param name="IdReparacion">Identificador de la Reparación.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> EliminarTipoReparacion(int IdReparacion);

        /// <summary>
        /// Obtiene el ViewModel del Grupo
        /// </summary>
        /// <returns>ViewModel para el Taller especifico.</returns>
        List<GrupoReparacionViewModel> ObtenerGrupoReparacion();
    }
}
