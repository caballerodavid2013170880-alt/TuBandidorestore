using System.Collections.Generic;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using static SUVAN.BackOffice.Database.Entities.Marca;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IMarcaService
    {
        /// <summary>
        /// Agrega una nueva marca al sistema.
        /// </summary>
        /// <param name="model">Modelo con los datos de la marca.</param>
        /// <returns>True si la marca fue agregada correctamente, false en caso contrario.</returns>
        Task<bool> AgregarMarca(Database.Entities.Marca model);
        Task EliminarMarca(short idMarca);

        /// <summary>
        /// Elimina una marca existente por su ID.
        /// </summary>
        /// <param name="idMarca">ID único de la marca.</param>
        /// <returns>True si la marca fue eliminada exitosamente, false si no se encontró.</returns>
        /// 


        // Task<bool> EliminarMarca(int idMarca);

        /// <summary>
        /// Obtiene todas las marcas registradas en el sistema.
        /// </summary>
        /// <returns>Lista de marcas disponibles.</returns>
        /// 

        Task<List<Database.Entities.Marca>> GetMarcas();

        /// <summary>
        /// Obtiene los datos de una marca específica por su ID.
        /// </summary>
        /// <param name="id">ID único de la marca.</param>
        /// <returns>El modelo de la marca si existe, null en caso contrario.</returns>
        Task<Database.Entities.Marca?> GetMarcaViewModel(int id);
    }
}