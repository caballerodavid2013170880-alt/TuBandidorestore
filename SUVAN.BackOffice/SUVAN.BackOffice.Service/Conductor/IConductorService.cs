using SUVAN.BackOffice.Models.ActivacionUsuario;
using SUVAN.BackOffice.Models.ActualizaFotografia;
using SUVAN.BackOffice.Models.ActualizaPassword;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.Conductor;
using SUVAN.BackOffice.Models.Corrida;
using SUVAN.BackOffice.Models.GeneraCodigo;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Mensajeria;
using SUVAN.BackOffice.Models.RecuperaPassword;
using SUVAN.BackOffice.Models.Viajes;

namespace SUVAN.BackOffice.Service.Conductor
{
  public interface IConductorService
  {
    Task<SuVanResponse<string>> Registro(ConductorRegistroRequest data);

    Task<SuVanResponse<string>> Firebase(int UserID, string firebaseid);


    Task<SuVanResponse<string>> SolicitaActivacion(SolicitaActivacionRequest data);

    Task<SuVanResponse<string>> Activacion(ActivacionUsuarioRequest data);

    Task<SuVanResponse<string>> GeneraCodigo(GeneraCodigoRequest data);

    Task<SuVanResponse<string>> RecuperaPassword(RecuperaPasswordRequest data);

    Task<SuVanResponse<string>> ActualizaPassword(ActualizaPasswordRequest data);

    Task<SuVanResponse<PerfilConductorModel>> ObtenerPerfil(int userId);

    Task<SuVanResponse<ActualizaPerfilRequest>> ActualizaPerfil(int userId, ActualizaPerfilRequest model, string code);

    Task<SuVanResponse<string>> ActualizaFotografia(int userId, ActualizaFotografiaRequest data);

    Task<Database.Entities.Conductor> getInfoEmail(string email);

    Task<Database.Entities.Conductor> getInfo(string email, string password);

    Task<Database.Entities.Conductor> getInfoID(int ConductorID);

    Task<SuVanResponse<List<ConductorCorridas>>> ProximasCorridas(int conductorId);

    Task<SuVanResponse<CorridaCalificacion>> CalificaCorrida(CorridaCalificacion data);

    Task<SuVanResponse<List<RutaCorridaAsignada>>> RutasCorridaAsignada(int conductorId);

    Task<SuVanResponse<List<ConductorCorridas>>> BusquedaCorridasAsignadas(BusquedaCorridasRequest data, int conductorId);

    Task<SuVanResponse<CalificacionesViajeResponse>> ConductorCalificaciones(int corrida_asignacionId);

    Task<SuVanResponse<EstadisticasConductorResponse>> EstadisticasConductor(int conductorId, EstatidisticasConductorRequest data);

    Task<int> GeneraRecibo(int operadorID, string fechaInicio, string fechaFin);

    Task<byte[]> EmiteRecibo(int liquidacionID);
    Task<SuVanResponse<List<AdministradorConversacionModel>>> ObtineAdministradoresPorEmpresa(int empresaId);
  }
}
