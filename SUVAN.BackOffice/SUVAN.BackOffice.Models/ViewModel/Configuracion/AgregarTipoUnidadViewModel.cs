using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  public class AgregarTipoUnidadViewModel
  {
    public int TipoUnidadId { get; set; }

    [Required(ErrorMessage = "El Nombre es requerido")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Los Asientos es requerido")]
    public int Asientos { get; set; } = 0;

    public bool Activo { get; set; } = true;


  }
}
