using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface IParadasService
  {
    /// <summary>
    /// Agrega o actualiza una estación en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel de la estación a agregar o actualizar.</param>
    /// <returns>Indica si la operación fue exitosa.</returns>
    /// <exception cref="Exception">Excepción lanzada en caso de errores.</exception>
    Task<bool> AgregarEstacion(AgregarEstacionViewModel model);
    /// <summary>
    /// Obtiene el ViewModel para la estación específica.
    /// </summary>
    /// <param name="id">Identificador de la estación.</param>
    /// <returns>ViewModel de la estación.</returns>
    Task<AgregarEstacionViewModel> GetEstacionViewModel(int id);
    /// <summary>
    /// Obtiene el listado de estaciones desde la base de datos.
    /// </summary>
    /// <returns>Listado de estaciones.</returns>
    Task<List<Paradum>> GetParadas();
    Task<List<Paradum>> GetParadasByName(string name);
  }
}