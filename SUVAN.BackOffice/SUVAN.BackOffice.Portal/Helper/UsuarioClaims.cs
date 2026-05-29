using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Helper
{


  public static class UsuarioClaims
  {
    const string claimTypePerfilId = "Perfil";
    const string claimTypeEmpresaId = "Empresa";
    const string claimTypeNombreEmpresa = "NombreEmpresa";

    const string claimTypeId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

    public static int GetEmpresaId(this ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {
        var userEmpresa = user.Claims
          .FirstOrDefault(i => i.Type == claimTypeEmpresaId)!.Value;

        return int.Parse(userEmpresa);
      }

      return 0;
    }

    public static string GetNombreEmpresa(this ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {
        var userEmpresa = user.Claims
          .FirstOrDefault(i => i.Type == claimTypeNombreEmpresa)!.Value;

        return userEmpresa;
      }

      return string.Empty;
    }

    public static string GetPerfilId(this ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {
        var userPerfil = user.Claims
          .FirstOrDefault(i => i.Type == claimTypePerfilId)!.Value;

        return userPerfil;
      }

      return null!;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {
        var userId = user.Claims
          .FirstOrDefault(i => i.Type == claimTypeId)!.Value;

        return int.Parse(userId);
      }

      return 0;
    }
  }
}
