using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  public class AgregarPoliticaCompensacionViewModel
  {
    public int PoliticaId { get; set; }
    public int EmpresaId { get; set; }
    public int TipoPolitca { get; set; }
    public int TipoCancelacion { get; set; }
    public string? Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rango de timepo es requerido")]
    public decimal RangoTiempo { get; set; }
    public int TipoTiempo { get; set; }
    [Required(ErrorMessage = "El porcentaje es requerido")]
    public decimal PorcentajeCompensacion { get; set; }
    public bool Activo { get; set; } = true;

  }
}
