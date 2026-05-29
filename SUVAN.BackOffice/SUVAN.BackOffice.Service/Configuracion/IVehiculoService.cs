using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoEspecificacionesViewModel;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface IVehiculoService
  {
    /// <summary>
    /// Agrega o actualiza un vehículo en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel del vehículo.</param>
    /// <returns>Indica si la operación fue exitosa.</returns>
    /// <exception cref="Exception">Se lanza en caso de error.</exception>
    Task<bool> AgregarVehiculo(AgregarUnidadViewModel model, int empresaId);
    /// <summary>
    /// Obtiene todos los vehículos desde la base de datos, incluyendo la información del tipo de vehículo asociado.
    /// </summary>
    /// <returns>Lista de todos los vehículos.</returns>
    Task<List<Vehiculo>> GetAllVehiculos(int empresaId);
    /// <summary>
    /// Obtiene el ViewModel para agregar/editar un vehículo.
    /// </summary>
    /// <param name="id">Identificador del vehículo.</param>
    /// <returns>ViewModel para agregar/editar un vehículo.</returns>
    Task<AgregarUnidadViewModel> GetVehiculoViewModel(int id, int empresaId);

    Task<int> AgregarDetalle(AgregarUnidadViewModel model, int idvehiculo);

    Task<List<ModeloUnidadViewModel>> ObtenerModelo(int? marcaId);
  }
}