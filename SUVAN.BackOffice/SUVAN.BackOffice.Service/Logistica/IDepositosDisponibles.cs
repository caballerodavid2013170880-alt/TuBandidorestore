using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IDepositosDisponibles
    {
        /// <summary>
        /// Agrega o actualiza un depósito en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del depósito.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarDeposito(DepositosDisponiblesViewModel model);

        /// <summary>
        /// Obtiene el listado de Depositos desde la base de datos.
        /// </summary>
        /// <returns>Lista de Depositos.</returns>
        Task<List<Depositosdisponible>> GetDepositos();

        /// <summary>
        /// Obtiene el ViewModel del depósito específico.
        /// </summary>
        /// <param name="id">Identificador del deposito.</param>
        /// <returns>ViewModel para el depósito especifico.</returns>
        Task<DepositosDisponiblesViewModel> GetDepositoViewModel(int id);


        /// <summary>
        /// Elimina un depósito en la base de datos.
        /// </summary>
        /// <param name="DepositoId">Identificador del deposito.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> EliminarDeposito(int DepositoId);
    }
}
