using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
  public class OlvidoContraViewModel
  {
    [Required(ErrorMessage = "El Correo es requerido")]
    public string Email { get; set; } = null!;
  }
}
