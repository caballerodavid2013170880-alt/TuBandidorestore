using System.Collections.Generic;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Database.Entities;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IModeloService
    {
        Task<List<Modelo>> GetModelo();

        /// <summary>
        /// Obtiene el ViewModel del Modelo específico.
        /// </summary>
        /// <param name="id">Identificador del modelo.</param>
        /// <returns>ViewModel para el modelo especifico.</returns>
        Task<ModeloViewModel> GetModeloViewModel(int id);

        /// <summary>
        /// Agrega o actualiza una modelo en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del modelo.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarModelo(ModeloViewModel model);

        /// <summary>
        /// Elimina un modelo en la base de datos.
        /// </summary>
        /// <param name="IdModelo">Identificador del modelo.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        Task<bool> EliminarModelo(int IdModelo);

        List<TipoVehiculoViewModel> ObtenerTipoVehiculo();

        List<MarcaViewModel> ObtenerMarca();
    }
}