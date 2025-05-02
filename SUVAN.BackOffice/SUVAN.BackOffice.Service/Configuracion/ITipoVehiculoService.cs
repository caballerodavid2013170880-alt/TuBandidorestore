using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface ITipoVehiculoService
  {


    /// <summary>
    /// Agrega o actualiza un tipo de vehiculo en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel con los datos del tipo de vehiculo.</param>
    /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
    /// <exception cref="Exception"></exception>
    Task<bool> AgregarTipoVehiculo(AgregarTipoUnidadViewModel model);

    /// <summary>
    /// Obtiene los tipos de vehiculos desde la base de datos.
    /// </summary>
    /// <returns>Lista de tipos de vehiculos.</returns>
    Task<List<Tipovehiculo>> GetTipovehiculos();
    Task<List<Tipovehiculo>> GetTipovehiculosActivo();

    /// <summary>
    /// Obtiene el ViewModel para agregar/editar un tipo de vehiculo.
    /// </summary>
    /// <param name="id">Identificador del tipo de vehiculo.</param>
    /// <returns>ViewModel para agregar/editar un tipo de vehiculo.</returns>
    Task<AgregarTipoUnidadViewModel> GetTipoVehiculoViewModel(int id);
  }
}