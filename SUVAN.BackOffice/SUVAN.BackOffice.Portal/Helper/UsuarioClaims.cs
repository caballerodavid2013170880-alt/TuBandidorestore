using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Helper
{


  public static class UsuarioClaims
  {
    const string claimTypePerfilId = "Perfil";
    const string claimTypeEmpresaId = "Empresa";
    const string claimTypeNombreEmpresa = "NombreEmpresa";
    const string claimTypeNombreDeposito = "NombreDeposito";

        const string claimTypeRegionId = "RegionId";
        const string claimTypePlantaId = "PlantaId";
        const string claimTypeZonaId = "ZonaId";
        const string claimTypeDepositoId = "DepositoId";
        const string claimTypeDeptoId = "DeptoId";


        const string claimTypeId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

    public static int GetEmpresaId(this ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {
        var userEmpresa = user.Claims
          .FirstOrDefault(i => i.Type == claimTypeEmpresaId)?.Value;

                return string.IsNullOrEmpty(userEmpresa) ? 0 : int.Parse(userEmpresa);
            }

      return 0;
    }

    public static string GetNombreEmpresa(this ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {
        var userEmpresa = user.Claims
          .FirstOrDefault(i => i.Type == claimTypeNombreEmpresa)?.Value;

            return userEmpresa ?? string.Empty;
        }


            return string.Empty;
        }

    public static string GetNombreDeposito(this ClaimsPrincipal user)
        {
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userDeposito = user.Claims
                  .FirstOrDefault(i => i.Type == claimTypeNombreDeposito)?.Value;

                return userDeposito ?? string.Empty;
            }
            return string.Empty;
    }

    public static string GetPerfilId(this ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {
        var userPerfil = user.Claims
            .FirstOrDefault(i => i.Type == claimTypePerfilId)?.Value;

        return userPerfil!;
      }

      return null!;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
      if (user.Identity!.IsAuthenticated)
      {
        var userId = user.Claims
          .FirstOrDefault(i => i.Type == claimTypeId)?.Value;

          return string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
      }

      return 0;
    }


        public static int? GetRegionId(this ClaimsPrincipal user)
        {
            return GetClaimIntNullable(user, claimTypeRegionId);
        }

        public static int? GetPlantaId(this ClaimsPrincipal user)
        {
            return GetClaimIntNullable(user, claimTypePlantaId);
        }

        public static int? GetZonaId(this ClaimsPrincipal user)
        {
            return GetClaimIntNullable(user, claimTypeZonaId);
        }

        public static int? GetDepositoId(this ClaimsPrincipal user)
        {
            return GetClaimIntNullable(user, claimTypeDepositoId);
        }

        public static int? GetDeptoId(this ClaimsPrincipal user)
        {
            return GetClaimIntNullable(user, claimTypeDeptoId);
        }

        private static int? GetClaimIntNullable(ClaimsPrincipal user, string claimType)
        {
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var claim = user.Claims.FirstOrDefault(i => i.Type == claimType);
                if (claim != null && int.TryParse(claim.Value, out int result))
                {
                    return result;
                }
            }
            return null;
        }




    }
}
