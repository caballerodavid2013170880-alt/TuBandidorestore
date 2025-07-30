using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  public class AgregarUnidadViewModel
  {
    public int UnidadId { get; set; }
    [Required(ErrorMessage = "Las Placas es requerida")]
    public string Placas { get; set; } = null!;
    public string? Vin { get; set; } = null;

    public bool Activo { get; set; } = true;

    public string TipoUnidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El Tipo de unidad es requerido")]
    public int TipoUnidadId { get; set; }

    public List<TipoUnidadViewModel> TipoUnidades { get; set; } = new List<TipoUnidadViewModel>();

    public string NumeroPoliza { get; set; } = string.Empty;
    public DateTime? FechaFinSeguro { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string NumeroEconomico { get; set; } = string.Empty;
    public string NumeroMotor { get; set; } = string.Empty;

    public List<ServicioUnidadViewModel> Servicios { get; set; } = new List<ServicioUnidadViewModel>();
    public string ServiciosJson { get; set; } = string.Empty;
        
    public int? IdMarca { get; set; }
    public int? IdModelo { get; set; }
    public List<MarcaUnidadViewModel> Marcas { get; set; } = new List<MarcaUnidadViewModel>();
    public List<ModeloUnidadViewModel> Modelos { get; set; } = new List<ModeloUnidadViewModel>();
    public string MarcaJson { get; set; } = string.Empty;
    }

  public class TipoUnidadViewModel
  {
    public int TipoUnidadId { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Seleccionado { get; set; } = false;
  }

  public class ServicioUnidadViewModel
  {
    public int servicioUnidadId { get; set; }
    public int unidadId { get; set; }
    public string detalle { get; set; } = null!;
    public DateTime fechaServicio { get; set; }
  }

    public class MarcaUnidadViewModel
    {
        public int? IdMarca { get; set; }
        public string DescripcionMarca { get; set; } = null!;
        public List<ModeloUnidadViewModel> Modelos { get; set; } = new();
    }

    public class ModeloUnidadViewModel
    {
        public int? IdModelo { get; set; }
        public string DescripcionModelo { get; set; } = null!;
    }

}
