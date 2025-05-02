using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.MensajeriaService
{
    public interface IConversacionesService
    {
        public Task<List<BandejaConversacionesModel>> ConsultaBandeja(int usuarioAdmin, int? conversacionId);
        public Task<SuVanResponse<TipoConversacion>> ObtenerTipoConversacion(int tipo,int? rutaId, int? operadorId);
        public Task<SuVanResponse<ConversacionModel>>  NuevaConversacion(ConversacionModel model);
        public Task<SuVanResponse<int>> ModificarEstatus(int conversacionId, int estatus);
        public Task<SuVanResponse<List<ConversacionModel>>> ConsultaConversaciones(int usuarioAdmin);
        public Task<SuVanResponse<List<ConversacionModel>>> ConsultaConversacionesUsuario(int usuario_id);
        public Task<SuVanResponse<ConversacionConexionModel>> ConsultaIdConexion(int conversacion_id);
        public Task RegistraMensaje(int conversacionId, int userId, int tipoUsuario, string mensaje);
        public Task RegistraActualizaConexion(int conversacionId, string conexionId);
        public Task<SuVanResponse<ConversacionModel>> ObtenerConversacion(MensajeConversacion mode);
    }
}
