using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using static SUVAN.BackOffice.Models.StoredsProcedures.ModelsStoredsProcedures;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface IRutasService
  {
    /// <summary>
    /// Agrega o actualiza una ruta en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel de la ruta a agregar o actualizar.</param>
    /// <returns>Indica si la operación fue exitosa.</returns>
    /// <exception cref="Exception">Excepción lanzada en caso de errores.</exception>
    Task<bool> AgregarRuta(AgregarRutaViewModel model, int empresaId);
    Task<bool> EliminarRuta(int rutaId, int empresaId);
    Task<List<ModelRutaConfiguracion>> GetRutasSinConfigurar(int empresaId);
    Task<List<ModelRutaConfiguracion>> GetRutasSinConfigurarSoloRed(int empresaId);

    /// <summary>
    /// Obtiene el ViewModel para la ruta específica.
    /// </summary>
    /// <param name="id">Identificador de la ruta.</param>
    /// <returns>ViewModel de la ruta.</returns>
    Task<AgregarRutaViewModel> GetRutaViewModel(int id, int empresaId);

    /// <summary>
    /// Obtiene la lista de rutas desde la base de datos.
    /// </summary>
    /// <returns>Lista de rutas.</returns>
    Task<List<Rutum>> ObtenerRutas(int empresaId);
  }
}