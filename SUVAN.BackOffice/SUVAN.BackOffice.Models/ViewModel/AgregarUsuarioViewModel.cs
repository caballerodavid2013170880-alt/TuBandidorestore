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
    // Campos de asignación jerárquica
    public int? IdRegion { get; set; }
    public int? IdPlanta { get; set; }
    public int? IdZona { get; set; }
    public int? IdDeposito { get; set; }
    public int? IdDepto { get; set; }

    public List<PerfilViewModel> Perfiles { get; set; } = new List<PerfilViewModel>();
    public List<EmpresaViewModel> Empresas { get; set; } = new List<EmpresaViewModel>();

    public List<EmpresaUsuarioViewModel> EmpresasSeleccion { get; set; } = new List<EmpresaUsuarioViewModel>();

    // Colecciones para desplegables en cascada
    public List<CatalogItemViewModel> Regiones { get; set; } = new List<CatalogItemViewModel>();
    public List<CatalogItemViewModel> Plantas { get; set; } = new List<CatalogItemViewModel>();
    public List<CatalogItemViewModel> Zonas { get; set; } = new List<CatalogItemViewModel>();
    public List<CatalogItemViewModel> Depositos { get; set; } = new List<CatalogItemViewModel>();
    public List<CatalogItemViewModel> Departamentos { get; set; } = new List<CatalogItemViewModel>();

    // Arreglo JSON de múltiples jerarquías por depósito
    public string JerarquiasUsuario { get; set; } = string.Empty;
    public List<UsuarioJerarquiaItemViewModel> JerarquiasSeleccion { get; set; } = new List<UsuarioJerarquiaItemViewModel>();
    }



public class UsuarioJerarquiaItemViewModel
    {
    public int idUsuarioJerarquia { get; set; }
    public int empresaId { get; set; }
    public int? regionId { get; set; }
    public string regionNombre { get; set; } = string.Empty;
    public int? plantaId { get; set; }
    public string plantaNombre { get; set; } = string.Empty;
    public int? zonaId { get; set; }
    public string zonaNombre { get; set; } = string.Empty;
    public int? depositoId { get; set; }
    public string depositoNombre { get; set; } = string.Empty;
    public int? deptoId { get; set; }
    public string deptoNombre { get; set; } = string.Empty;
    public bool esPrincipal { get; set; }
    }

public class CatalogItemViewModel
    {
    public int Id { get; set; }
    public string? Nombre { get; set; }
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