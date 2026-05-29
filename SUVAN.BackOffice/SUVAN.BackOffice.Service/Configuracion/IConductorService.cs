using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface IConductorService
  {

    /// /// <summary>
    /// Agrega o actualiza un conductor en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel con los datos del conductor.</param>
    /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
    /// <exception cref="Exception"></exception>
    Task<bool> AgregarConductor(AgregarConductorViewModel model, int empresaId);

    /// <summary>
    /// Obtiene todos los conductores desde la base de datos.
    /// </summary>
    /// <returns>Lista de conductores.</returns>
    Task<List<Database.Entities.Conductor>> GetConductores(int empresaId);
    /// <summary>
    /// Obtiene el ViewModel para el conductor específico.
    /// </summary>
    /// <param name="id">Identificador del conductor.</param>
    /// <returns>ViewModel para el conductor específico.</returns>
    Task<AgregarConductorViewModel> GetConductorViewModel(int id, int empresaId);
    Task<List<RegimenFiscalViewModel>> GetRegimenFiscal();
    Task<List<ReporteConductorViewModel>> ReporteOperadores();
  }
}