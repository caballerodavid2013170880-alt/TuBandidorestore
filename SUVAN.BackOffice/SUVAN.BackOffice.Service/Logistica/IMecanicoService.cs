using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IMecanicoService
    {
        /// <summary>
        /// Obtiene el listado de los Mecanicos desde la base de datos.
        /// </summary>
        /// <returns>Lista de Mecanicos</returns>
        Task<List<Mecanico>> GetMecanico();

        /// <summary>
        /// Obtiene el ViewModel del mecanico específico.
        /// </summary>
        /// <param name="id">Identificador del mecanico.</param>
        /// <returns>ViewModel para el mecanico especifico.</returns>
        Task<MecanicoViewModel> GetMecanicoViewModel(int id);

        /// <summary>
        /// Agrega o actualiza un mecanico en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del mecanico.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarMecanico(MecanicoViewModel model);

        /// <summary>
        /// Elimina un mecanico en la base de datos.
        /// </summary>
        /// <param name="MecanicoId">Identificador del mecanico.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarMecanico(int MecanicoId);

        List<DepositosDisponiblesViewModel> ObtenerDeposito();
    }
}
