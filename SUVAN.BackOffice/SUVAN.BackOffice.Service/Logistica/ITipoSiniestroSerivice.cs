/*
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ITipoSiniestroService
    {
        /// <summary>
        /// Obtiene el listado de los Tipos de Siniestros desde la base de datos.
        /// </summary>
        /// <returns>Lista de Tipos de Siniestros.</returns>
        Task<List<TipoSiniestro>> GetTipoSiniestro();

        /// <summary>
        /// Obtiene el ViewModel de un siniestro específico.
        /// </summary>
        /// <param name="id">Identificador del siniestro.</param>
        /// <returns>ViewModel para el siniestro específico.</returns>
        Task<CausaSiniestroViewModel.TipoSiniestroViewModel> GetTipoSiniestroViewModel(int id);

        /// <summary>
        /// Agrega o actualiza un siniestro en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del siniestro.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarTipoSiniestro(CausaSiniestroViewModel.TipoSiniestroViewModel model);

        /// <summary>
        /// Elimina un siniestro en la base de datos.
        /// </summary>
        /// <param name="IdTipoSiniestro">Identificador del siniestro.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> EliminarTipoSiniestro(int IdTipoSiniestro);
    }
}
*/