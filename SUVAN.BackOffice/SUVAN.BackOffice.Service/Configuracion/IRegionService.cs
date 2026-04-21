using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
    public interface IRegionService
    {

        /// <summary>
        /// Agrega o actualiza una region en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la region.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarRegion(RegionViewModel model);

        /// <summary>
        /// Obtiene el listado de Regiones desde la base de datos.
        /// </summary>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <returns>Lista de Regiones.</returns>
        Task<List<Region>> GetRegiones(int id_empresa);

        /// <summary>
        /// Obtiene el ViewModel para la región específica.
        /// </summary>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <param name="id_region">Identificador de la región.</param>
        /// <returns>ViewModel para la región específica.</returns>
        Task<RegionViewModel> GetRegionViewModel(int id_empresa, int id_region);
    }
}
