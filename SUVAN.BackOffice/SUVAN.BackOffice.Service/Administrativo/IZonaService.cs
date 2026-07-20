using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface IZonaService
    {
        /*/// <summary>
        /// Obtiene el listado de las Zonas desde la base de datos.
        /// </summary>
        /// <returns>Lista de Zonas.</returns>
        Task<List<Zona>> GetZona(int IdEmpresa);

        /// <summary>
        /// Obtiene el ViewModel de la zona específica.
        /// </summary>
        /// <param name="id">Identificador de la zona.</param>
        /// <returns>ViewModel para la zona especifica.</returns>
        Task<ZonaViewModel> GetZonaViewModel(int id, int IdEmpresa);

        /// <summary>
        /// Agrega o actualiza una zona en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la zona.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarZona(ZonaViewModel model, int IdEmpresa);

        /// <summary>
        /// Elimina una zona en la base de datos.
        /// </summary>
        /// <param name="IdZona">Identificador de la zona.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarZona(int IdZona);

        Task<List<ZonaViewModel.CatalogItemViewModel>> ObtenerPlantasPorRegion(int idEmpresa, int idRegion);
        */

        //Remake Cat Zona
        /// <summary>
        /// Obtiene el listado de Zonas de la empresa indicada, incluyendo navegación a Región y Planta.
        /// </summary>
        Task<List<Zona>> GetZona(int idEmpresa);

        /// <summary>
        /// Construye el ViewModel para el formulario de alta o edición de una zona.
        /// </summary>
        Task<ZonaViewModel> GetZonaViewModel(int idEmpresa, int idZona);

        /// <summary>
        /// Agrega o actualiza una zona, validando jerarquía Región -> Planta.
        /// </summary>
        Task<bool> AgregarZona(ZonaViewModel model, int idEmpresa);

        /// <summary>
        /// Elimina lógicamente o físicamente una zona.
        /// </summary>
        Task<bool> EliminarZona(int IdZona, int idEmpresa);
    }
}
