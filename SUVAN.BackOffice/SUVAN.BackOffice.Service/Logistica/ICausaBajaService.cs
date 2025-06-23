using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ICausaBajaService
    {
        Task<List<CausaBaja>> GetCausaBaja();

        /// <summary>
        /// Obtiene el ViewModel de la Causa Baja del Vehiculo específico.
        /// </summary>
        /// <param name="idCausa">Identificador de la Causa Baja del Vehiculo.</param>
        /// <returns>ViewModel de la Baja especifica.</returns>
        Task<CausaBajaViewModel> GetCausaBajaViewModel(int idCausa);

        /// <summary>
        /// Agrega o actualiza una causa en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la causa.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarCausaBajaVehiculo(CausaBajaViewModel model);

        /// <summary>
        /// Elimina una causa en la base de datos.
        /// </summary>
        /// <param name="IdCausa">Identificador de la causa.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> EliminarCausaBajaVehiculo(int IdCausa);

        public List<BajaVehiViewModel> ObtenerBajaVehiculo();
    }
}
