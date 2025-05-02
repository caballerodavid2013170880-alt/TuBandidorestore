using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.AtributosModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  [UnicaHoraInicio(ErrorMessage = "No pueden existir dos registros con la misma hora de inicio.")]
  public class AgregarCorridaViewModel
  {

    public int RutaId { get; set; }
    public string NombreRuta { get; set; } = string.Empty;

    public List<CorridaViewModel> Corridas { get; set; } = new();


  }

  public class CorridaViewModel
  {

    public int CorridaId { get; set; }
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
    public TimeOnly Inicio { get; set; }

    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
    [HoraFinMayorQueInicio(nameof(Inicio), ErrorMessage = "La hora de fin debe ser mayor que la hora de inicio.")]

    public TimeOnly Fin { get; set; }
    [AnyDiasActivo(ErrorMessage = "Al menos un día debe estar activo.")]
    public List<DiasViewModel> Dias { get; set; } = new();


  }

  public class DiasViewModel
  {

    public string DiaId { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool Activo { get; set; }
  }

  public class DeleteCorridaViewModel
  {
    public int RutaId { get; set; } = 0;

  }


  public class DetalleRutaViewModel
  {
    public int CorridaId { get; set; } = 0;
    public List<EstacionViewModel> Estaciones { get; set; } = new();

    public int CantidadEstaciones { get; set; } = 0;
  }

  public class EstacionViewModel
  {
    public int ParadaId { get; set; }
    public string NombreEstacion { get; set; } = string.Empty;

    public TimeOnly Horario { get; set; }
    public int Tiempo { get; set; }
  }
}
