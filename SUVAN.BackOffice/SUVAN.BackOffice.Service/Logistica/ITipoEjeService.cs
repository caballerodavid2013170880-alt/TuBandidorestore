using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ITipoEjeService
    {
        Task<List<TipoEje>> GetTipoEje();

        /// <summary>
        /// Obtiene el ViewModel del Tipo de Eje específico.
        /// </summary>
        /// <param name="idTipoEje">Identificador del tipo de eje.</param>
        /// <returns>ViewModel del tipo de eje especifico </returns>
       Task<TipoEjeViewModel> GetTipoEjeViewModel(int idTipoEje);

        /// <summary>
        /// Agrega o actualiza un tipo de eje en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del tipo de eje.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarTipoEje(TipoEjeViewModel model);

        /// <summary>
        /// Elimina una Tipo de Eje en la base de datos.
        /// </summary>
        /// <param name="IdTipoEje">Identificador del Tipo de Eje.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> EliminarTipoEje(int IdTipoEje);
    }
}
