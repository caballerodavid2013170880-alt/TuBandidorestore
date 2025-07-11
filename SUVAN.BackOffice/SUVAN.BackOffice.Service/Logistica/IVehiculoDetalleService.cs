using System.Collections.Generic;
using System.Threading.Tasks;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IVehiculoDetalleService
    {
        Task<List<VehiculoDetalle>> GetVehiculoDetalle();

        /// <summary>
        /// Obtiene el ViewModel del VehiculoDetalle específico.
        /// </summary>
        /// <param name="id">Identificador del VehiculoDetalle.</param>
        /// <returns>ViewModel para el VehiculoDetalle especifico.</returns>
        Task<VehiculoDetalleViewModel> GetVehiculoDetalleViewModel(int id);

        /// <summary>
        /// Agrega o actualiza un registro Vehiculo Detalle en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de Vehiculo Detalle.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarVehiculoDetalle(VehiculoDetalleViewModel model, int IdEmpresa, string Usuario);

        /// <summary>
        /// Elimina un registro de Vehiculo Detalle en la base de datos.
        /// </summary>
        /// <param name="IdVehiculoDetalle">Identificador del Vehiculo Detalle.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarVehiculoDetalle(int IdVehiculoDetalle);

        Task CompletarCamposError(VehiculoDetalleViewModel model);

        Task<List<VehiculoDetalleViewModel>> ObtenerDetalleModal(int idVehiculoDetalle);
    }

}