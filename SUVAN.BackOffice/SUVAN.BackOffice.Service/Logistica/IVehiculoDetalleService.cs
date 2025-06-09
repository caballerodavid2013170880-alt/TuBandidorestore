using System.Collections.Generic;
using System.Threading.Tasks;
using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IVehiculoDetalleService
    {
        /// <summary>
        /// Agrega un nuevo vehículo al sistema.
        /// </summary>
        /// <param name="model">Modelo con los datos del vehículo.</param>
        /// <returns>True si el vehículo fue agregado correctamente, false en caso contrario.</returns>
        Task<bool> AgregarVehiculo(VehiculoDetalle model);

        /// <summary>
        /// Elimina un vehículo existente por su ID.
        /// </summary>
        /// <param name="idVehicDetalle">ID único del vehículo.</param>
        /// <returns>True si el vehículo fue eliminado exitosamente, false si no se encontró.</returns>
        Task<bool> EliminarVehiculo(int idVehicDetalle);

        /// <summary>
        /// Obtiene todos los vehículos registrados en el sistema.
        /// </summary>
        /// <returns>Lista de vehículos disponibles.</returns>
        Task<List<VehiculoDetalle>> GetVehiculos();

        /// <summary>
        /// Obtiene los datos de un vehículo específico por su ID.
        /// </summary>
        /// <param name="id">ID único del vehículo.</param>
        /// <returns>El modelo del vehículo si existe, null en caso contrario.</returns>
        Task<VehiculoDetalle?> GetVehiculoViewModel(int id);
    }
}