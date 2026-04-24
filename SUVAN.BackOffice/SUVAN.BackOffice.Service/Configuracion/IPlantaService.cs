using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
    public interface IPlantaService
    {
        /// <summary>
        /// Agrega o actualiza una planta en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la planta.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        Task<bool> AgregarPlanta(PlantaViewModel model);

        /// <summary>
        /// Obtiene el listado de Plantas desde la base de datos.
        /// </summary>
        /// <param name="id_emp">Identificador de la empresa.</param>
        /// <returns>Lista de Plantas.</returns>
        Task<List<Plantum>> GetPlantas(int id_emp);

        /// <summary>
        /// Obtiene el ViewModel para la planta específica.
        /// </summary>
        /// <param name="id_emp">Identificador de la empresa.</param>
        /// <param name="id_region">Identificador de la región.</param>
        /// <param name="id_planta">Identificador de la planta.</param>
        /// <returns>ViewModel para la planta específica.</returns>
        Task<PlantaViewModel> GetPlantaViewModel(int id_emp, int id_region, int id_planta);
    }
}