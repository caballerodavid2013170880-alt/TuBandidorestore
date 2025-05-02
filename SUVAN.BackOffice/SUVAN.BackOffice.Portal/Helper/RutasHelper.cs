using SUVAN.BackOffice.Service.Configuracion;
using static SUVAN.BackOffice.Models.StoredsProcedures.ModelsStoredsProcedures;

namespace SUVAN.BackOffice.Portal.Helper
{
  public class RutasHelper : IRutasHelper
  {
    private readonly IRutasService rutasService;

    public RutasHelper(IRutasService rutasService)
    {
      this.rutasService = rutasService;
    }


    public async Task<bool> ExisteRutaSinConfigurar(int empresaId)
    {
      var rutas = await rutasService.GetRutasSinConfigurarSoloRed(empresaId);
      return rutas.Any();
    }

    public async Task<List<ModelRutaConfiguracion>> ObtenerRutasSinConfigurar(int empresaId)
    {
      return await rutasService.GetRutasSinConfigurarSoloRed(empresaId);
    }
  }
}
