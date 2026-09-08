using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Portal.Helper;

namespace SUVAN.BackOffice.Portal.Controllers
{
  [Controller]
  public abstract class BaseController : Controller
  {
        /// <summary>
        /// Obtiene las variables de contexto de sesión del usuario autenticado (incluyendo los Claims de Jerarquía).
        /// </summary>
        protected (int idEmpresa, int idUsuario, int? idRegion, int? idPlanta, int? idZona, int? idDeposito, int? idDepto) GetUserContext()
        {
            int idEmpresa = User.GetEmpresaId();
            int idUsuario = User.GetUserId();

            int? idRegion = User.GetRegionId();
            int? idPlanta = User.GetPlantaId();
            int? idZona = User.GetZonaId();
            int? idDeposito = User.GetDepositoId();
            int? idDepto = User.GetDeptoId();

            return (idEmpresa, idUsuario, idRegion, idPlanta, idZona, idDeposito, idDepto);
        }

    }
}
