using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.RegistroUsuario;
using SUVAN.BackOffice.Models.ActivacionUsuario;
using SUVAN.BackOffice.Models.GeneraCodigo;
using SUVAN.BackOffice.Models.RecuperaPassword;
using SUVAN.BackOffice.Models.ActualizaPassword;
using SUVAN.BackOffice.Models.ObjectParentResponse;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.ActualizaFotografia;
using SUVAN.BackOffice.Models.Viajes;


namespace SUVAN.BackOffice.Service.RegistroUsuario
{
    public interface IUsuarioService
    {

        Task<SuVanResponse<string>> Registro(RegistroUsuarioRequest data, string code);
        
        Task<SuVanResponse<string>> Firebase(int UserID, string firebaseid);

        Task<SuVanResponse<string>> Activacion(ActivacionUsuarioRequest data);

        Task<SuVanResponse<string>> GeneraCodigo(GeneraCodigoRequest data, string code);

        Task<SuVanResponse<string>> RecuperaPassword(RecuperaPasswordRequest data);

        Task<SuVanResponse<string>> ActualizaPassword(ActualizaPasswordRequest data);

        Task<Usuario> getInfoUsuario(string email, string password);
        Task<Usuario> getInfoUsuarioID(int UserID);

        Task<SuVanResponse<PerfilUsuarioModel>> ObtenerPerfil(int userId);
        Task<SuVanResponse<ActualizaPerfilRequest>> ActualizaPerfil(int userId, ActualizaPerfilRequest model, string code);
        Task<SuVanResponse<string>> ActualizaFotografia(int userId, ActualizaFotografiaRequest data);

        Task<Codigopai?> GetCodigopais(int? codigopais);

        Task<SuVanResponse<ViajeCalificacion>> CalificaConductor(ViajeCalificacion data);

    }


}
