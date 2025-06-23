using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IBajaVehiculoService
    {
        Task<List<BajaVehi>> GetBajaVehiculo();

        /// <summary>
        /// Obtiene el ViewModel de la Baja del Vehiculo específico.
        /// </summary>
        /// <param name="idBaja">Identificador de la Baja del Vehiculo.</param>
        /// <returns>ViewModel de la Baja especifica.</returns>
        Task<BajaVehiViewModel> GetBajaVehiculoViewModel(int idBaja);

        /// <summary>
        /// Agrega o actualiza una baja en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la baja.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarBajaVehiculo(BajaVehiViewModel model);

        /// <summary>
        /// Elimina una baja en la base de datos.
        /// </summary>
        /// <param name="IdBaja">Identificador de la baja.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarBajaVehiculo(int IdBaja);
    }
}
