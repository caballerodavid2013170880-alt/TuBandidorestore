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
    public interface IMotivoAuxilioVialService
    {
        /// <summary>
        /// Agrega o actualiza un Motivo de Auxilio Vial en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del Motivo de Auxilio.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarMotivoAuxilioVial(MotivoAuxilioVialViewModel model);

        /// <summary>
        /// Obtiene el listado de empresas desde la base de datos.
        /// </summary>
        /// <returns>Lista de empresas.</returns>
        Task<List<MotivoAuxilioVial>> GetMotivoAuxilioVial();

        /// <summary>
        /// Obtiene el ViewModel del Motivo de Auxilio específico.
        /// </summary>
        /// <param name="id">Identificador del Motivo de Auxilio.</param>
        /// <returns>ViewModel para el Motivo de Auxilio específico.</returns>
        Task<MotivoAuxilioVialViewModel> GetMotivoAuxilioViewModel(int id);

        /// <summary>
        /// Elimina un Motivo de Auxilio en la base de datos.
        /// </summary>
        /// <param name="MotivoId">Identificador del Motivo de Auxilio.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> EliminarMotivoAuxilioVial(int MotivoId);
    }
}
