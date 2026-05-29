using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public interface IAdminService
  {
    Task<Admin> ActivarAdmin(ActivarUsuarioViewModel model, bool esActivar = true);
    Task<Usuario> ActivarUsuarioApp(ActivarUsuarioViewModel model, bool esActivar = true);
    Task<Database.Entities.Conductor> ActivarUsuarioAppCond(ActivarUsuarioViewModel model, bool esActivar = true);
    Task<int> AgregarUsuario(AgregarUsuarioViewModel model);
    Task<Admin> AutenticarUsuario(string email, string password);
    Task<int> CambiarEmpresa(int adminId, int empresaId);
    Task<bool> EliminarAdmin(int adminId);
    Task<Admin> GetAdmin(string email);
    Task<Admin> GetAdmin(int id);
    Task<List<Admin>> GetAdmins();
    Task<AgregarUsuarioViewModel> GetAdminViewModel(int adminId);
    Task<List<AdminEmpresa>> GetEmpresaUsuario(int adminId);
    Task<bool> OlvidoContra(string email);
    Task<bool> OlvidoContraUsuarioApp(string email);
  }
}
