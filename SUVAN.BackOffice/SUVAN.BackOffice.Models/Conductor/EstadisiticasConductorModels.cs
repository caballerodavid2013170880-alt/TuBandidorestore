using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Conductor
{
    public class EstadisiticasConductorFrecuenciaDia
    {
        public DateTime Fecha { get; set; }
    }

    public class EstadisiticasConductorFrecuenciaMes
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
    }

    public class EstatidisticasConductorRequest
    {
        public int Frecuencia { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
    }

    public class DetalleEstadisiticas
    {
        public string Fecha { get; set; }
        public int Pasajeros { get; set; }
        public int Viajes { get; set; }
        public decimal Kilometros { get; set; }
        public decimal Ingresos { get; set; }
    }

    public class EstadisticasConductorResponse
    {
        public int Pasajeros { get; set; }
        public int Viajes { get; set; }
        public decimal Kilometros { get; set; }
        public decimal Ingresos { get; set; }
        public List<DetalleEstadisiticas> Detalle { get; set; }
    }
}
