using Microsoft.IdentityModel.Tokens;
using SUVAN.BackOffice.Models.Auth.Token;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.ObjectUsuarios;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SUVAN.BackOffice.API.Provider
{
    public class JwtAuthProvider : IJwtAuthProvider
    {
        private readonly IConfiguration _configuration;
        public JwtAuthProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<SuVanResponse<LoginResponse>> GenerateToken(LoginRequest request, String id, String email)
        {
            SuVanResponse<LoginResponse> response = new();
            LoginResponse? result = new LoginResponse();

            var _jwtSettings = _configuration.GetSection("JWTSettings").Get<JWTSettings>();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var dueDate = DateTime.Now.AddMinutes(_jwtSettings.ExpireTime);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,request.UserName,null),
                new Claim(ClaimTypes.Role,"UserMobile", null),
                new Claim("userId", id, null),
                new Claim("email", email, null)
            };
            var jwtSecurityToken = new JwtSecurityToken(_jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: dueDate,
                signingCredentials: credentials);

            var accessToken= new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            result.ExpiryTime = (_jwtSettings.ExpireTime * 60);
            result.AccessToken = accessToken;
            result.RefreshToken = GenerateRefreshToken(); // Por el momento se deja con el mismo HCV
            result.ExpirationDate = dueDate;
            response.Mensaje = "Solicitud exitosa";
            response.CodigoMensaje = "200";
            response.Data = result;
            return response;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
