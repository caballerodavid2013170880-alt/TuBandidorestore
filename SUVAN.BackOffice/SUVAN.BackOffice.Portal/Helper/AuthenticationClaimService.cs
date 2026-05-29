using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SUVAN.BackOffice.Database.Entities;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Helper
{
  public class AuthenticationClaimService : IAuthenticationClaimService
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthenticationClaimService(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// singin del usuario
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="empresa"></param>
    /// <returns></returns>
    public async Task SignInAsync(Admin usuario, AdminEmpresa empresa)
    {
      var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Idadmin.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre!),
            new Claim(ClaimTypes.Email, usuario.Email!),
            new Claim("Perfil", $"{empresa.PerfilIdperfil}"),
            new Claim("Empresa", $"{empresa!.EmpresaIdempresa!}"),
            new Claim("NombreEmpresa", $"{empresa!.EmpresaIdempresaNavigation.Nombre!}"),
            new Claim("Activo", $"{usuario.Activo}")
        };

      var claimsIdentity = new ClaimsIdentity(claims, "AuthScheme");

      await _httpContextAccessor.HttpContext!.SignInAsync("AuthScheme",
          new ClaimsPrincipal(claimsIdentity),
          new AuthenticationProperties());
    }

    /// <summary>
    /// logout del usuario
    /// </summary>
    /// <returns></returns>
    public async Task LogoutAsync()
    {
      await _httpContextAccessor.HttpContext!
              .SignOutAsync("AuthScheme");
    }
  }

}
