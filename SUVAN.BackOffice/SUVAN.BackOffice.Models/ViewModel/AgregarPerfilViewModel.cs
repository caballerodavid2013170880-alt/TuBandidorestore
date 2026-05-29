using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
  public class AgregarPerfilViewModel : IValidatableObject
  {
    public int PerfilId { get; set; }

    [Required(ErrorMessage = "El Nombre del pefil es requerido")]
    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; } = true;

    public List<OpcionPerfilViewModel> Opciones { get; set; } = new List<OpcionPerfilViewModel>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      // Verifica si al menos una de las propiedades en cada elemento de la lista es true
      if (!Opciones.Any(opcion => opcion.Agregar || opcion.Editar || opcion.Eliminar || opcion.Ejecutar))
      {
        yield return new ValidationResult("Al menos una opción de menú debe ser seleccionada (Agregar, Editar, Eliminar, Ejecutar)", new[] { "Opciones" });
      }
    }
  }

  public class OpcionPerfilViewModel
  {
    public int MenuId { get; set; }
    public string MenuNombre { get; set; } = string.Empty;
    public string MenuSeccion { get; set; } = string.Empty;
    public bool Agregar { get; set; } = false;
    public bool Editar { get; set; } = false;
    public bool Eliminar { get; set; } = false;
    public bool Ejecutar { get; set; } = false;
  }

  public class DeletePerfilViewModel
  {
    public int PerfilId { get; set; } = 0;

  }
}