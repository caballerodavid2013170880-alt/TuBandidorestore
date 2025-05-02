using SUVAN.BackOffice.Models.Corrida;
using SUVAN.BackOffice.Models.Parada;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Conductor
{
    public class ConductorCorridas
    {
        public sbyte? IdEstatusCorridaAsignacion { get; set; }

        public int idCorridaAsignacion {  get; set; }

        public int idRuta {  get; set; }

        public string ruta { get; set; }

        public DateOnly? fecha { get; set; }

        public TimeOnly? horaInicio { get; set; }

        public TimeOnly? horaFin { get; set; }

        public int pasajeros { get; set; }

        public ParadaCorrida paradaInicio { get; set; }

        public ParadaCorrida paradaTermino { get; set; }

        public List<CorridaEstacion> Estaciones { get; set; }

        public int? Calificacion { get; set; }
        public string? MensajeCalificacion { get; set; }

    }

    public class CorridaCalificacion
    {
        public int CorridaId { get; set; }

        public int Calificacion { get; set; }

        public string Mensaje { get; set; }
    }

    public class RutaCorridaAsignada 
    {
        public int idRuta { get; set; }

        public string ruta { get; set; }

        public List<string> horarios { get; set; }
    }

    public class BusquedaCorridasRequest 
    {
        public int idRuta {  set; get; }

        public TimeOnly? horario { get; set;}

        public DateOnly? fecha { get; set;}
    }


    public class ConductorCalificacionesResponse
    {
        public int? calificacion { get; set; }
        public string? comentarios { get; set; }
    }

    public class CalificacionesViajeResponse
    {
        public List<ConductorCalificacionesResponse> Conductor { get; set; }
        public List<ConductorCalificacionesResponse> Usuarios { get; set; }


    }
}
