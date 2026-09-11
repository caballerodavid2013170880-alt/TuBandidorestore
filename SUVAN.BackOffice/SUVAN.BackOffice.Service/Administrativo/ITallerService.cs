using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface ITallerService
    {
        /// <summary>
        /// Obtiene el listado de Talleres desde la base de datos.
        /// </summary>
        /// <returns>Lista de Talleres.</returns>
        Task<List<Taller>> GetTaller(int IdEmpresa);

        /// <summary>
        /// Obtiene el ViewModel del taller específico.
        /// </summary>
        /// <param name="id">Identificador del taller.</param>
        /// <returns>ViewModel para el taller especifico.</returns>
        Task<TallerViewModel> GetTallerViewModel(int id, int IdEmpresa);

        /// <summary>
        /// Agrega o actualiza un taller en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del taller.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarTaller(TallerViewModel model, int idEmpresa);

        // 1407 evitar conflictos con depostios disponibles: List<TallerViewModel.DepositosViewModel> ObtenerDeposito(int zonaId);

        /// <summary>
        /// Elimina un taller en la base de datos.
        /// </summary>
        /// <param name="TallerId">Identificador del taller.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarTaller(int TallerId);

        /// <summary>
        /// Metodos en cascada para la jerarquia del taller
        /// </summary>
        /// <returns>Cascada Jerarquia.</returns>

        //Casacada jerarquia
        Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetRegions(int id_empresa);
        Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetPlantasByRegion(int id_empresa, int id_region);
        Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetZonasByPlanta(int id_empresa, int id_planta);
        Task<List<VehiculoDetalleViewModel.CatalogItemViewModel>> GetDepositosByZona(int id_empresa, int id_zona);
    }
}
