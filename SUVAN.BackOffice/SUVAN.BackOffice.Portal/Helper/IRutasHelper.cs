
using SUVAN.BackOffice.Models.StoredsProcedures;

namespace SUVAN.BackOffice.Portal.Helper
{
  public interface IRutasHelper
  {
    Task<bool> ExisteRutaSinConfigurar(int empresaId);
    Task<List<ModelsStoredsProcedures.ModelRutaConfiguracion>> ObtenerRutasSinConfigurar(int empresaId);
  }
}
