using System;

namespace SUVAN.BackOffice.Models.ViewModel.Comercial
{
    public class ViajesViewModel
    {
        public ObtenerViajesViewModel Viajes { get; set; }
        public ObtenerDetalleViajesViewModel DetalleViajes { get; set; }
        public ViajesViewModel()
        {
            this.Viajes = new ObtenerViajesViewModel();
            this.DetalleViajes = new ObtenerDetalleViajesViewModel();
        }
    }
    public class ObtenerViajesViewModel
    {
        public int IdCorridaAsignacion { get; set; }
        public int CorridaIdCorrida { get; set; }
        public int ConductorIdConductor { get; set; }
        public int VehiculoIdVehiculo { get; set; }
        public DateTime? Fecha { get; set; }
        public int? EstatusViajeIdEstatusViaje { get; set; }
        public int? IdEstacionActual { get; set; }
        public string? ConductorNombre { get; set; }
        public int? TotalPasajeros { get; set; }
        public string? Nombre { get; set; }
    }

    public class ObtenerViajesUsuarioViewModel
    {
        public int ViajeId { get; set; }
        public string? Viaje { get; set; }
        public string? Nombre { get; set; }
        public string? Codigo { get; set; }
        public int? Pasajeros { get; set; }
        public DateTime? Fechaviaje { get; set; }
    }
    public class ObtenerCorridasViewModel
    {
        public int IdCorridaAsignacion { get; set; }
        public int CorridaIdCorrida { get; set; }
        public int ConductorIdConductor { get; set; }
        public int VehiculoIdVehiculo { get; set; }
        public DateTime? Fecha { get; set; }
        public int? EstatusViajeIdEstatusViaje { get; set; }
        public int? IdEstacionActual { get; set; }
        public int? TotalPasajeros { get; set; }
    }
    public class ObtenerDetalleViajesViewModel
    {
        public int ViajeId { get; set; }
        public string? Viaje { get; set; }
        public string? Ruta { get; set; }
        public string? Operador { get; set; }
        public int? Usuarios { get; set; }
        public int Tramos { get; set; }
        public List<ParadaRutasViewModel> Estaciones { get; set; } = new List<ParadaRutasViewModel>();
        public int IdRuta { get; set; }
        public int? CorridaAsignacionId { get; set; }
        public int? empresaId { get; set; }
        public int? estatusId { get; set; }
        public DateTime? Fechaviaje { get; set; }
    }
    public class ParadaRutasViewModel
    {
        public int RuataIdRuta { get; set; }
        public string? ParadaNombre { get; set; }
        public int order { get; set; }
    }

    public class ViajeCancelacionViewModel
    {
        public int CorridaAsignacionId { get; set; }
        public int ViajeId { get; set; }
        public int TrnasaccionId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioFirebaseId { get; set; } = string.Empty;
        public decimal Cantidad { get; set; } = 0;
        public int EstatusTransaccion { get; set; } = 0;

    }

    public class ViajePoliticaRangoPorcentaje
    {
        public decimal RangoTiempo { get; set; } = 0;
        public decimal Porcentaje { get; set; } = 0;
    }
}

