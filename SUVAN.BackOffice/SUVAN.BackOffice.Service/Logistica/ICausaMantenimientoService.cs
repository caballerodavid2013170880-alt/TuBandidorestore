using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ICausaMantenimientoService
    {
        Task<List<CausaMantenimiento>> GetCausaMantenimiento();

        /// <summary>
        /// Obtiene el ViewModel de la causa específica.
        /// </summary>
        /// <param name="id">Identificador de la causa</param>
        /// <returns>ViewModel para la causa específica.</returns>
        Task<CausaMantenimientoViewModel> GetCausaMantenimientoViewModel(int id);

        /// <summary>
        /// Agrega o actualiza una causa en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la causa.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarCausaMantenimiento(CausaMantenimientoViewModel model);

        /// <summary>
        /// Elimina una causa de mantenimiento en la base de datos.
        /// </summary>
        /// <param name="IdCausa">Identificador de la causa de mantenimiento.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarCausaMantenimiento(int IdCausa);


    }
}
