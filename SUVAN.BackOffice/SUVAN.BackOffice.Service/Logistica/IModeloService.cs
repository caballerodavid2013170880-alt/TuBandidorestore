using System.Collections.Generic;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IModeloService
    {
        /// <summary>
        /// Agrega un nuevo modelo al sistema.
        /// </summary>
        /// <param name="model">Modelo con los datos del modelo.</param>
        /// <returns>True si el modelo fue agregado correctamente, false en caso contrario.</returns>
        Task<bool> AgregarModelo(Modelo model);

        /// <summary>
        /// Elimina un modelo existente por su ID.
        /// </summary>
        /// <param name="idModelo">ID único del modelo.</param>
        /// <returns>True si el modelo fue eliminado exitosamente, false si no se encontró.</returns>
        Task EliminarModelo(short idModelo);

        /// <summary>
        /// Obtiene todos los modelos registrados en el sistema.
        /// </summary>
        /// <returns>Lista de modelos disponibles.</returns>
        Task<List<Modelo>> GetModelos();

        /// <summary>
        /// Obtiene los datos de un modelo específico por su ID.
        /// </summary>
        /// <param name="id">ID único del modelo.</param>
        /// <returns>El modelo si existe, null en caso contrario.</returns>
        Task<Modelo?> GetModeloViewModel(int id);
    }
}