using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IFallaAuxilioVial
    {
        /// <summary>
        /// Agrega o actualiza una Falla de Auxilio Vial en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de Falla de Auxilio.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarFallaAuxilioVial(FallaAuxilioVialViewModel model);

        /// <summary>
        /// Obtiene el listado de Fallas de Auxilio Vial desde la base de datos.
        /// </summary>
        /// <returns>Lista de Fallas de Auxilio.</returns>
        Task<List<FallaAuxilioVial>> GetFallaAuxilioVial();

        /// <summary>
        /// Obtiene el ViewModel del Motivo de Auxilio específico.
        /// </summary>
        /// <param name="id">Identificador del Motivo de Auxilio.</param>
        /// <returns>ViewModel para el Motivo de Auxilio específico.</returns>
        Task<FallaAuxilioVialViewModel> GetFallaAuxilioViewModel(int id);

        /// <summary>
        /// Elimina un Motivo de Auxilio en la base de datos.
        /// </summary>
        /// <param name="FallaId">Identificador del Motivo de Auxilio.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        /// 
        Task<bool> EliminarFallaAuxilioVial(int FallaId);
    }
}

