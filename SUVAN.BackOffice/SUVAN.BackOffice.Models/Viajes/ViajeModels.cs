using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Viajes
{
    public class ViajeRutaModels
    {

        public int? ViajeRelacion { get; set; }
        public int CorridaAsignacionId { get; set; }

        public int ReservacionId { get; set; }
        public int RutaId { get; set; }
        public string? RutaNombre { get; set; }

        public string? GoogleMapsRuta { get; set; }

        public string? Costo { get; set; }
        public string? CostoPromocion { get; set; }
        public string? EstacionAbordaje { get; set; }
        public string? EstacionDescenso { get; set; }
        public int EstacionAbordajeId { get; set; }
        public int EstacionDescensoId { get; set; }

        [Required]
        public decimal LatitudInicial { get; set; }
        [Required]
        public decimal LongitudInicial { get; set; }
        [Required]
        public decimal LatitudFinal { get; set; }
        [Required]
        public decimal LongitudFinal { get; set; }


        public string? OrigenNombre { get; set; }
        public string? OrigenDireccion { get; set; }

        public string? DestinoNombre { get; set; }
        public string? DestinoDireccion { get; set; }

        public int EstatusViajeId { get; set; }


    }

    public class ViajeServicioRequest
    {
        [Required]
        public decimal LatitudInicial { get; set; }
        [Required]
        public decimal LongitudInicial { get; set; }
        [Required]
        public decimal LatitudFinal { get; set; }
        [Required]
        public decimal LongitudFinal { get; set; }
    }

    public class ViajeServicioResponse
    {
        public int RutaId { get; set; }
        public string? RutaNombre { get; set; }
        public string? Costo { get; set; }
        public string? CostoPromocion { get; set; }
        public string? EstacionAbordaje { get; set; }
        public string? EstacionDescenso { get; set; }
        public int EstacionAbordajeId { get; set; }
        public int EstacionDescensoId { get; set; }
        public string? EstacionInicial { get; set; }
        public string? EstacionIntermedia { get; set; }
        public string? EstacionFinal { get; set; }
    }

    public class ViajeDisponibilidadRequest
    {
        [Required]
        public int RutaId { get; set; }
        [Required]
        public int Pasajeros { get; set; }
        [Required]
        public string? Fecha { get; set; }
        [Required]
        public int EstacionAbordajeId { get; set; }
        [Required]
        public int EstacionDescensoId { get; set; }
    }

    public class ViajeDisponibilidadResponse
    {
        public int RutaId { get; set; }
        public string? RutaNombre { get; set; }
        public string? Costo { get; set; }
        public string? CostoPromocion { get; set; }
        public string? EstacionAbordaje { get; set; }
        public string? EstacionDescenso { get; set; }
        public int EstacionAbordajeId { get; set; }
        public int EstacionDescensoId { get; set; }
        public string? HoraAbordaje { get; set; }
        public string? HoraDescenso { get; set; }
        public int CorridaId { get; set; }
        public int AsientosPorVehiculo { get; set; }
        public int AsientosDisponibles { get; set; }
    }

    public class ViajeRutaResponse : ViajeRutaModels
    {
        public string? FechaAbordaje { get; set; }
        public string? HoraAbordaje { get; set; }

        public DateTime? FechaAbordajeDate { get; set; }

    }

    public class ViajeCalificacion
    {
        public int ReservacionId { get; set; }

        public int Calificacion { get; set; }

        public string Mensaje { get; set; }
    }


    public class ObtenDatosDeViajePorIdDeVajeModel
    {
        public int CorridaAsignacionId { get; set; }
        public int CorridaId { get; set; }

        public int RutaId { get; set; }

        public int EmpresaId { get; set; }

        public int TipoTarifaId { get; set; }

        public int ParadaInicio { get; set; }

        public int ParadaFin { get; set; }

    }

    public class ObtenDatosUsuarioPorIdDeViajeModel
    {
        public int CorridaAsignacionId { get; set; }
        public int CorridaId { get; set; }
        public int usuarioId { get; set; } 
    }

    public class CancelacionViaje
    {
        public decimal? SaldoMonedero { get; set; }
    }

    public class ViajeFechasRequest
    {
        [Required]
        public int RutaId { get; set; }
        [Required]
        public int EstacionAbordajeId { get; set; }
        [Required]
        public int EstacionDescensoId { get; set; }
    }

    public class ViajesServicioFechaResponse
    {
        public DateTime? FechaViaje { get; set; }
    }

    public class ViajeOffline
    {
        public List<BoletoViajeResponse> Boleto { get; set; } = new List<BoletoViajeResponse>();
        //public List<ViajeRutaResponse> ProximosViajes { get; set; } = new List<ViajeRutaResponse>();
    }
}
