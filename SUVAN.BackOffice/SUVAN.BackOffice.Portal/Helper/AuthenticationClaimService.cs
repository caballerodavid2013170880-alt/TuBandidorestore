using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Service.Seguridad;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Helper
{
  public class AuthenticationClaimService : IAuthenticationClaimService
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsuarioJerarquiaService _usuarioJerarquiaService;
        public AuthenticationClaimService(IHttpContextAccessor httpContextAccessor, IUsuarioJerarquiaService usuarioJerarquiaService)
    {
      _httpContextAccessor = httpContextAccessor;
      _usuarioJerarquiaService = usuarioJerarquiaService;
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

            // Consultar y adjuntar Claims de Jerarquía si existen
            var jerarquia = await _usuarioJerarquiaService.GetJerarquiaUsuario("Admin", usuario.Idadmin, empresa.EmpresaIdempresa);
            if (jerarquia != null)
            {
                if (jerarquia.IdRegion.HasValue) claims.Add(new Claim("RegionId", jerarquia.IdRegion.Value.ToString()));
                if (jerarquia.IdPlanta.HasValue) claims.Add(new Claim("PlantaId", jerarquia.IdPlanta.Value.ToString()));
                if (jerarquia.IdZona.HasValue) claims.Add(new Claim("ZonaId", jerarquia.IdZona.Value.ToString()));
                if (jerarquia.IdDeposito.HasValue) claims.Add(new Claim("DepositoId", jerarquia.IdDeposito.Value.ToString()));
                if (jerarquia.IdDepto.HasValue) claims.Add(new Claim("DeptoId", jerarquia.IdDepto.Value.ToString()));
            }

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
