using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.ObjectUsuarios;

namespace SUVAN.BackOffice.API.Provider
{
    public interface IJwtAuthProvider
    {
        Task<SuVanResponse<LoginResponse>> GenerateToken(LoginRequest request, String id, String email);
    }
}
