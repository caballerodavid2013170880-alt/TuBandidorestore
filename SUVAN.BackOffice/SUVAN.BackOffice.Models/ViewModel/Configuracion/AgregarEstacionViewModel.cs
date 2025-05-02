using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  public class AgregarEstacionViewModel
  {
    public int EstacionId { get; set; }

    [Required(ErrorMessage = "El nombre de la estación es requerido")]
    public string NombreEstacion { get; set; } = null!;


    [Required(ErrorMessage = "La latitud es requerida")]
    public decimal Latitud { get; set; } = 0;

    [Required(ErrorMessage = "La longitud es requerida")]
    public decimal Longitud { get; set; } = 0;

    [Required(ErrorMessage = "La calle es requerida")]
    public string Calle { get; set; } = null!;

    public string Numero { get; set; } = string.Empty;

    public string Municipio { get; set; } = string.Empty;

    public string Ciudad { get; set; } = string.Empty;
    public string Colonia { get; set; } = string.Empty;


    public string CodigoPostal { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
  }
}
