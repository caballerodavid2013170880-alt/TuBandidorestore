using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Viajes
{
    public class BoletoModel
    {
        public string? Codigo { get; set; }
        public string? QR { get; set; }
    }

    public class AutoModel
    {
        public string? Placas { get; set; }
        public string? Descripcion { get; set; }
        public string? Color { get; set; }
    }

    public class BoletoViajeRequest
    {
        public int ReservacionId { get; set; }
    }

    public class BoletoViajeResponse
    { 
        public string? Fecha { get; set; }
        public string? Hora { get; set; }
        public int IdcorridaAsignacion { get; set; }
        public int CorridaId { get; set; }
        public int Pasajeros { get; set; }
        public string? ConductorNombre { get; set; }
        public AutoModel? Auto { get; set; }
        public string? DireccionAbordaje { get; set; }
        public string? DireccionDescenso { get; set; }
        public string? EstacionAbordaje { get; set; }
        public string? EstacionDescenso { get; set; }
        public int EstacionAbordajeId { get; set; }
        public int EstacionDescensoId { get; set; }
        public string? HoraAbordaje { get; set; }
        public string? HoraDescenso { get; set; }
        public BoletoModel? Boleto { get; set; }
        public Decimal Monto { get; set; }
        public bool Calificado { get; set; }
        public int Calificacion { get; set; }
        public string? MensajeCalificacion { get; set; }
        public bool? Facturado { get; set; }
        public int EstatusViajeId { get; set; }
        public Models.Viajes.Viajeredondo ViajeRedondo { get; set; }
        public bool Cancelable { get; set; }
    }


}
