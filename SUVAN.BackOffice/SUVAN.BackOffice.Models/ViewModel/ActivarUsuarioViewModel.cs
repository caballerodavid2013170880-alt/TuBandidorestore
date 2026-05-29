using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
  public class ActivarUsuarioViewModel
  {
    [Required(ErrorMessage = "El Correo es requerido")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "La Contraseña es requerida")]
    public string Password { get; set; } = null!;
    [Required(ErrorMessage = "La Confirmación de la Contraseña es requerida")]
    public string ConfirmarPassword { get; set; } = null!;
  }
}
