using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  public class AgregarRutaViewModel
  {
    public int RutaId { get; set; }
    [Required(ErrorMessage = "El nombre de la ruta es requerido")]
    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; } = true;
    public string GoogleMapsRuta { get; set; } = string.Empty;
    public int Distancia { get; set; } = 0;

    List<PuntosViewModel> Puntos { get; set; } = new List<PuntosViewModel>();

    [Required(ErrorMessage = "Es necesario agregar estaciones a la ruta")]
    public string PuntosJson { get; set; } = string.Empty;
  }

  public class PuntosViewModel
  {
    public int id { get; set; }
    public string label { get; set; } = null!;
    public int order { get; set; }
    public int tipo { get; set; }
    public int tiempo { get; set; }
    public List<CoordinatesViewModel> coordinatesArray { get; set; } = new List<CoordinatesViewModel>();
  }

  public class CoordinatesViewModel
  {
    public decimal lat { get; set; }
    public decimal lng { get; set; }
  }

  public class RutaConfiguracionViewModel
  {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Corrida { get; set; } = false;
    public bool CorridaHorarioEstacion { get; set; } = false;
    public bool AsignacionChofer { get; set; } = false;
    public bool Tarifa { get; set; } = false;
    public bool tarifaGen { get; set; } = false;

  }
}
