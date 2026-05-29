using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Dashboard
{
  public class ViajeIngresosOcupacionDashboard
  {
    public List<ViajeIngresosOcupacion> Chart { get; set; } = new();
    public List<ViajeIngresosOcupacionTable> Table { get; set; } = new();
  }
  public class ViajeIngresosOcupacion
  {

    public string Periodo { get; set; } = null!;
    public decimal CantidadUsuarios { get; set; }
    public decimal SumatoriaCostoFinal { get; set; }

  }

  public class ViajeIngresosOcupacionTable
  {
    public string FechaViaje { get; set; } = string.Empty;
    public string Boleto { get; set; } = string.Empty;
    public Decimal? Costo { get; set; }
    public int CantidadBoleto { get; set; }
    public decimal SumatoriaCostoFinal { get; set; }

  }

  public class ViajeIngresosOcupacionFiltro
  {
    public string Indicador { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public string Fecha { get; set; } = string.Empty;
    public string RutaId { get; set; } = string.Empty;
    public string HorarioId { get; set; } = string.Empty;
  }

  public class MensajesLeidosModel
  {

    public string ConversacionId { get; set; } = string.Empty;
  }

}
