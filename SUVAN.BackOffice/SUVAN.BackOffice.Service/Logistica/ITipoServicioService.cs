using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ITipoServicioService
    {
        /// <summary>
        /// Obtiene el listado de los Tipos de Servicios desde la base de datos.
        /// </summary>
        /// <returns>Lista de Tipos de Servicios.</returns
        Task<List<TipoServicio>> GetTipoServicio();

        /// <summary>
        /// Obtiene el ViewModel del servicio específico.
        /// </summary>
        /// <param name="id">Identificador del servicio.</param>
        /// <returns>ViewModel para el servicio especifico.</returns>
        Task<CausaMantenimientoViewModel.TipoServicioViewModel> GetTipoServicioViewModel(int id);

        /// <summary>
        /// Agrega o actualiza un servicio en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del servicio.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarTipoServicio(CausaMantenimientoViewModel.TipoServicioViewModel model);

        /// <summary>
        /// Elimina una servicio en la base de datos.
        /// </summary>
        /// <param name="IdServicio">Identificador del servicio.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarTipoServicio(int IdServicio);
    }
}
