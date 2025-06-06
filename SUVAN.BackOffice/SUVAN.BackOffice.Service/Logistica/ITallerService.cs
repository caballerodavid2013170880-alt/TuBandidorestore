using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ITallerService
    {
        /// <summary>
        /// Obtiene el listado de Talleres desde la base de datos.
        /// </summary>
        /// <returns>Lista de Talleres.</returns>
        Task<List<Taller>> GetTaller(int IdEmpresa);

        /// <summary>
        /// Obtiene el ViewModel del taller específico.
        /// </summary>
        /// <param name="id">Identificador del taller.</param>
        /// <returns>ViewModel para el taller especifico.</returns>
        Task<TallerViewModel> GetTallerViewModel(int id, int IdEmpresa);

        /// <summary>
        /// Agrega o actualiza un taller en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del taller.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarTaller(TallerViewModel model);

        List<TallerViewModel.DepositosViewModel> ObtenerDeposito(int zonaId);

        /// <summary>
        /// Elimina un taller en la base de datos.
        /// </summary>
        /// <param name="TallerId">Identificador del taller.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarTaller(int TallerId);
    }
}
