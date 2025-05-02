using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
  public class AgregarContenidoViewModel
  {
    public int ContenidoId { get; set; }
    [Required(ErrorMessage = "El Titulo es requerido")]
    public string Titulo { get; set; } = null!;
    [Required(ErrorMessage = "El Contenido es requerido")]
    public string Contenido { get; set; } = null!;

    public bool Activo { get; set; } = true;
    public int? Orden { get; set; }
    public int TipoContenidoId { get; set; }
    public IFormFile? Imagen { get; set; } = null;

    public string? Imagen64 { get; set; } = null;

  }

  public class DeleteContenidoViewModel
  {
    public int ContenidoId { get; set; } = 0;

  }

  public class OrdenarContenidoViewModel
  {
    public List<int> NuevoOrden { get; set; } = new List<int>();
  }
}
