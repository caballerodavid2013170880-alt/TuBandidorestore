using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Descuento;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Service.RegistroUsuario;

namespace SUVAN.BackOffice.Service.Descuento
{
    public class DescuentoService : IDescuentoService
    {
        private readonly SuvanDbContext _context;

        private readonly IUsuarioService _usuarioService;

        public DescuentoService(SuvanDbContext context, IUsuarioService usuarioService)
        {
            _context = context;
            _usuarioService = usuarioService;
        }

        public async Task<SuVanResponse<DescuentoCodigoResponse>> ValidarCodigo(int userId, DescuentoCodigoRequest data)
        {
            SuVanResponse<DescuentoCodigoResponse> response = new();

            DescuentoCodigoResponse? result = await (from o in _context.Codigodescuentos
                                                  where o.Codigo == data.Codigo
                                                  && o.Vigenciadesde <= DateTime.Now && o.Vigenciahasta >= DateTime.Now
                                                  && o.Activo == 1
                                                  select new DescuentoCodigoResponse()
                                                  {
                                                      Descuento = o.Cantidad ?? 0,
                                                      TipoDescuento = o.TipodescuentoIdtipodescuento1 == 1 ? "$" : "%"
                                                  }).FirstOrDefaultAsync();

            response.Data = result;
            response.CodigoMensaje = "200";
            response.Mensaje = "Codigo de promoción valido";
            if (result == null) {
                response.CodigoMensaje = "400";
                response.Mensaje = "Codigo de promoción invalido";
            }

            return response;
        }
    }
}
