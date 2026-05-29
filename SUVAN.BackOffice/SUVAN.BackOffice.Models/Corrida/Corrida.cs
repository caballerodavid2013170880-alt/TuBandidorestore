using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Corrida
{

    public class BoletoCheckRequest
    {
        [Required(ErrorMessage = "Falta el boleto")]
        public string Boleto { get; set; }
    }

    public class CorridaUsuario
    {
        public int Idviaje { get; set; }

        public int Idusuario { get; set; }

        public string Nombre { get; set; }

        public string Boleto { get; set; }

        public string Firebase { get; set; }


        public int? Pasajeros { get; set; }

        public DateTime Fechacheck { get; set; }

        public int? Status { get; set; }

    }

    public class CorridaParadaRequest
    {
        public int Idcorrida { get; set; }

        public int Idparada { get; set; }
    }

    public class CorridaParadaResponse
    {
        public int? Idcorrida { get; set; }

        public int Idparada { get; set; }

        public int? pasajeros { get; set; }

        public List<CorridaUsuario> Suben { get; set; }

        public List<CorridaUsuario> Bajan { get; set; }
    }

    public class CorridaRequest
    {
        public int Idcorrida { get; set; }
    }

    public class CorridaEstacionesResponse
    {
        public int Idcorrida { get; set; }

        public int Idruta { get; set; }

        public string Ruta { get; set; }

        public DateTime? Fecha { get; set; }

        public TimeOnly? HoraInicio { get; set; }

        public TimeOnly? HoraFin { get; set; }

        public int Pasajeros { get; set; }

        public CorridaEstacion ParadaInicio { get; set; }

        public CorridaEstacion ParadaTermino { get; set; }

        public CorridaEstacion Actual { get; set; }

        public List<CorridaEstacion> Estaciones { get; set; }

    }


    public class PasajerosSubenBajan
    {
        public int Suben { get; set; }
        public int Bajan { get; set; }

    }
    public class CorridaEstacion
    {
        public int IdCorridaAsignacion { get; set; }
        public int Idruta { get; set; }
        public int Idparada { get; set; }

        public string Nombre { get; set; }

        public string Direccion { get; set; }

        public decimal? Latitud { get; set; }

        public decimal? Longitud { get; set; }

        public int? IdEstatus { get; set; }

        public int? Orden { get; set; }

		public int? Abordan { get; set; }

		public int? Bajan { get; set; }

		public int? SiguienteAbordan { get; set; }

        public int? SiguienteBajan { get; set; }
        public bool Ultima { get; set; }

		public bool EsEstacion { get; set; }


    }

    public class AbordajeRequest
    {
        [Required(ErrorMessage = "Falta el identificador de la corrida")]
        public int Idcorrida { get; set; }

        [Required(ErrorMessage = "Falta el identificador de la parada")]
        public int Idparada { get; set; }

    }

    public class ComenzarAbordajeRequest : AbordajeRequest
    {

        [Required(ErrorMessage = "Falta la longitud de la posición de la unidad")]
        public decimal Longitud { get; set; }

        [Required(ErrorMessage = "Falta la latitud de la posición de la unidad")]
        public decimal Latitud { get; set; }
    }
}
