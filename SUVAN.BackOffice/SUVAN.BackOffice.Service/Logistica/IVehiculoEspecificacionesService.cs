using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoEspecificacionesViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IVehiculoEspecificacionesService
    {
        Task<List<MarcaEspecifiViewModel>> ObtenerMarcas();

        Task<List<ModeloEspecifiViewModel>> ObtenerModelo(int marcaId);

        Task<VehiculoEspecificacionesViewModel> GetVehiculoEspecifi();

        Task<VehiculoEspecificacionesViewModel> ObtenerEspecificaciones(VehiculoEspecificacionesViewModel model);


        /// <summary>
        /// Obtiene el ViewModel del VehiculoEspecificaciones específico.
        /// </summary>
        /// <param name="id">Identificador del VehiculoEspecificaciones.</param>
        /// <returns>ViewModel para el VehiculoEspecificaciones especifico.</returns>
        Task<VehiculoEspecificacionesViewModel> GetVehiculoEspecifiViewModel(int id);

        /// <summary>
        /// Agrega o actualiza un registro Vehiculo Especificaciones en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de Vehiculo Especificaciones.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarVehiculoEspecificaciones(VehiculoEspecificacionesViewModel model);
    }
}
