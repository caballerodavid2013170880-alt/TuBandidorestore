using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IZonaService
    {
        /// <summary>
        /// Obtiene el listado de las Zonas desde la base de datos.
        /// </summary>
        /// <returns>Lista de Zonas.</returns>
        Task<List<Zona>> GetZona();

        /// <summary>
        /// Obtiene el ViewModel de la zona específica.
        /// </summary>
        /// <param name="id">Identificador de la zona.</param>
        /// <returns>ViewModel para la zona especifica.</returns>
        Task<ZonaViewModel> GetZonaViewModel(int id);

        /// <summary>
        /// Agrega o actualiza una zona en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la zona.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarZona(ZonaViewModel model);

        /// <summary>
        /// Elimina una zona en la base de datos.
        /// </summary>
        /// <param name="IdZona">Identificador de la zona.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarZona(int IdZona);
    }
}
