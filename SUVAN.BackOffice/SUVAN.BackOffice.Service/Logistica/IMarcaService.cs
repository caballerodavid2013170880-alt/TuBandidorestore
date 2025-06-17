using System.Collections.Generic;
using System.Threading.Tasks;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using static SUVAN.BackOffice.Database.Entities.Marca;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IMarcaService
    {
        Task<List<Marca>> GetMarca();

        /// <summary>
        /// Obtiene el ViewModel de la marca específica.
        /// </summary>
        /// <param name="id">Identificador de la marca.</param>
        /// <returns>ViewModel para la marca especifica.</returns>
        Task<MarcaViewModel> GetMarcaViewModel(int id);

        /// <summary>
        /// Agrega o actualiza una marca en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la marca.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarMarca(MarcaViewModel model);

        /// <summary>
        /// Elimina una marca en la base de datos.
        /// </summary>
        /// <param name="IdMarca">Identificador de la marca.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> EliminarMarca(int IdMarca);
    }
}