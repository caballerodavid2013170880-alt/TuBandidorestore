
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  public class ChoferUnidadAgregarViewModel
  {
    public int RutaId { get; set; }
    public int CorridaId { get; set; }
    public List<ChoferUnidad> Conductores { get; set; } = new();
    public List<DateTime> Fechas { get; set; } = new();
  }

  public class ChoferUnidad
  {
    public int conductorId { get; set; }
    public string conductor { get; set; } = string.Empty;
    public int vehiculoId { get; set; }
    public string vehiculo { get; set; } = string.Empty;
    public bool Consulta { get; set; } = false;
  }

  public class ChoferUnidadViewModel
  {

    public int RutaId { get; set; }
    public int? HorarioId { get; set; }
    public int? IdHoSeleccionado { get; set; }

    public int CorridaId { get; set; }
    public int ConductorId { get; set; }
    public int VehiculoId { get; set; }
    public string RutasJson { get; set; } = string.Empty;
    public string FechasAsignacionJson { get; set; } = string.Empty;
    public List<RutasViewModel> Rutas { get; set; } = new();
    public List<ConductorViewModel> Conductores { get; set; } = new();
    public List<VehiculoVewModel> Vehiculos { get; set; } = new();

    public List<CorridaAsignacionViewModel> CorridaAsignacion { get; set; } = new();


  }

  public class CorridaAsignacionViewModel
  {
    public string Conductor { get; set; } = string.Empty;
    public string Vehiculo { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public List<DateTime> Fechas { get; set; } = new();
  }

  public class RutasViewModel
  {
    public int RutaId { get; set; }
    public string Nombre { get; set; } = null!;
    public List<CorridasRutaViewModel> Corridas { get; set; } = new();

  }

  public class CorridasRutaViewModel
  {
    public int CorridaId { get; set; }
    public string Horas { get; set; } = null!;
    public List<int> DiasInactivos { get; set; } = new();
    public List<string> FechasSeleccionadas { get; set; } = new();
  }

  public class ConductorViewModel
  {
    public int ChoferId { get; set; }
    public string Nombre { get; set; } = null!;
  }


  public class VehiculoVewModel
  {
    public int UnidadId { get; set; }
    public string Descripcion { get; set; } = null!;
  }

  public class CorridaReasingacionViewModel
  {
    public int CorridaAsignacionId { get; set; }
    public int RutaId { get; set; }
    public string NombreRuta { get; set; } = string.Empty;
    public string Conductor { get; set; } = string.Empty;
    public int ConductorId { get; set; }
    public string Vehiculo { get; set; } = string.Empty;
    public int VehiculoId { get; set; }
    public int HorarioId { get; set; }
    public string Horario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
  }

  public class ConsultaReasingacionViewModel
  {
    public int ConductorId { get; set; } = 0;
    public int VehiculoId { get; set; } = 0;
  }


  public class ReasignarChoferViewMolde
  {
    public int ConductorId { get; set; }
    public int VehiculoId { get; set; }
    public int ConductorReasignarId { get; set; }
    public int VehiculoReasignarId { get; set; }
    public List<ConductorViewModel> Conductores { get; set; } = new();
    public List<VehiculoVewModel> Vehiculos { get; set; } = new();
    public List<CorridaReasingacionViewModel> CorridaReasingacion { get; set; } = new();
  }


  public class ReasignarChoferViewModel
  {
    public int conductorReasignarId { get; set; } = 0;
    public int vehiculoReasignarId { get; set; } = 0;

    public List<ReasignarDetalleChoferViewModel> detalle { get; set; } = new();
  }
  public class ReasignarDetalleChoferViewModel
  {

    public int corridaAsingnacionId { get; set; } = 0;
    public int rutaId { get; set; } = 0;
    public string ruta { get; set; } = string.Empty;
    public int conductorId { get; set; } = 0;
    public string conductor { get; set; } = string.Empty;
    public int unidadId { get; set; } = 0;
    public string unidad { get; set; } = string.Empty;
    public int horarioId { get; set; } = 0;
    public string horario { get; set; } = string.Empty;
    public string fecha { get; set; } = string.Empty;


  }
}
