using SUVAN.BackOffice.Models.Descuento;
using SUVAN.BackOffice.Models.ManejoRespuesta;

namespace SUVAN.BackOffice.Service.Descuento
{
    public interface IDescuentoService
    {

        Task<SuVanResponse<DescuentoCodigoResponse>> ValidarCodigo(int userId, DescuentoCodigoRequest data);

    }
}
