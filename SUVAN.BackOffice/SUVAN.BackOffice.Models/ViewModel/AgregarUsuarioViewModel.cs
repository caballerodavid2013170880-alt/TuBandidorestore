using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel
{
  public class AgregarUsuarioViewModel
  {
    public int AdminId { get; set; }
    [Required(ErrorMessage = "El Nombre de usuario es requerido")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El Correo electrónico es requerido")]

    public string Email { get; set; } = string.Empty;
    //[Required(ErrorMessage = "La Contraseña es requerido")]

    public string Password { get; set; } = string.Empty;

    //[Required(ErrorMessage = "El perfil del usuario es requerido")]

    public int? PerfilId { get; set; }
    public int? EmpresaId { get; set; }


    [Required(ErrorMessage = "Se deben asignar empresas al usuario")]
    public string EmpresasUsuario { get; set; } = null!;

    public bool Activo { get; set; } = true;

    public List<PerfilViewModel> Perfiles { get; set; } = new List<PerfilViewModel>();
    public List<EmpresaViewModel> Empresas { get; set; } = new List<EmpresaViewModel>();

    public List<EmpresaUsuarioViewModel> EmpresasSeleccion { get; set; } = new List<EmpresaUsuarioViewModel>();
  }

  public class EmpresaUsuarioViewModel
  {
    public int empresaId { get; set; }
    public string empresaNombre { get; set; } = string.Empty;
    public int perfilId { get; set; }
    public string perfilNombre { get; set; } = string.Empty;
    public bool esPrincipal { get; set; }
  }

  public class PerfilViewModel
  {
    public int PerfilId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Seleccionado { get; set; } = false;
  }

  public class EmpresaViewModel
  {
    public int EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
  }

  public class DeleteUsuarioViewModel
  {

    public int UserId { get; set; }
  }
}
