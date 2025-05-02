using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Portal.Helper
{
  public interface IAuthenticationClaimService
  {
    Task LogoutAsync();
    Task SignInAsync(Admin usuario, AdminEmpresa empresa);
  }
}