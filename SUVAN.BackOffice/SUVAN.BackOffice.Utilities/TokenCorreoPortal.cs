using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Utilities
{
  public static class TokenCorreoPortal
  {
    private const string secretKey = "ThisIsCustomSecretKeyForAuthenticationSUVANBackOffice2024";

    /// <summary>
    /// Genera un jwt token para el correo de activacion
    /// </summary>
    /// <param name="email"></param>
    /// <param name="vigencia"></param>
    /// 
    /// <returns></returns>
    public static string GeneraToken(string email, int vigencia = 1)
    {
      string tokenResult = string.Empty;
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, email)
            }),
          Expires = DateTime.UtcNow.AddDays(vigencia),
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        tokenResult = tokenHandler.WriteToken(token).ToString();
      }
      catch (Exception)
      {
        throw;
      }
      return tokenResult;
    }

    /// <summary>
    /// validacion del token recibido
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static string ValidateToken(string token)
    {
      try
      {
        var handler = new JwtSecurityTokenHandler();
        var tokenResult = handler.ReadToken(token) as JwtSecurityToken;
        var tokenExpiryDate = tokenResult!.ValidTo.ToLocalTime();

        if (tokenExpiryDate >= DateTime.Now)
        {
          return tokenResult.Claims.First().Value;
        }

        throw new Exception("El Token ha expirado, Contactarse con el Administrador del Portal.");
      }
      catch (Exception)
      {
        throw;
      }
    }
  }
}
