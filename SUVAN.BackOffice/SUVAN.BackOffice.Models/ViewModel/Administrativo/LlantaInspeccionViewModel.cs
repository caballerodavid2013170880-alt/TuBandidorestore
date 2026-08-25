using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class LlantaInspeccionViewModel
    {
        public int IdVehiculo { get; set; }

        public DateTime FechaInspeccion { get; set; } = DateTime.Today;

        [ValidateNever]
        public List<LlantaInspeccionCatalogoViewModel> Vehiculos { get; set; } = new();

        [ValidateNever]
        public List<LlantaInspeccionCatalogoViewModel> TiposInspeccion { get; set; } = new();

        [ValidateNever]
        public List<LlantaInspeccionCatalogoViewModel> EstadosInspeccion { get; set; } = new();

        [ValidateNever]
        public List<LlantaInspeccionCatalogoViewModel> ConclusionesInspeccion { get; set; } = new();

        [ValidateNever]
        public List<LlantaInspeccionCatalogoViewModel> EstadosLlanta { get; set; } = new();

    }

    public class LlantaInspeccionCatalogoViewModel
    {
        public ulong Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }

    public class LlantaInspeccionGuardarViewModel
    {
        public string ContextoInspeccion { get; set; } = "Vehiculo";

        public int IdVehiculo { get; set; }

        public ulong IdTipoInspeccion { get; set; }

        public DateTime FechaInspeccion { get; set; }

        public uint? KilometrajeLlanta { get; set; }

        public List<LlantaInspeccionDetalleGuardarViewModel> Detalles { get; set; } = new();
    }

    public class LlantaInspeccionDetalleGuardarViewModel
    {
        public ulong IdLlantaAsignacion { get; set; }

        public ulong IdLlanta { get; set; }

        public decimal? ProfundidadMm { get; set; }

        public decimal? PresionPsi { get; set; }

        public ushort IdEstadoInspeccion { get; set; }

        public ushort IdConclusionInspeccion { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no deben exceder 1000 caracteres")]
        public string? Observaciones { get; set; }
    }

    public class LlantaInspeccionLlantaFueraVehiculoViewModel
    {
        public ulong IdLlanta { get; set; }

        public string CodigoLlanta { get; set; } = string.Empty;

        public string NumeroSerieDot { get; set; } = string.Empty;

        public string? Marca { get; set; }

        public string? Modelo { get; set; }

        public string? Medida { get; set; }

        public ushort IdEstadoLlanta { get; set; }

        public string EstadoLlanta { get; set; } = string.Empty;

        public uint? IdDeposito { get; set; }

        public string? Deposito { get; set; }

        public decimal? PresionMinimaPsi { get; set; }

        public decimal? PresionMaximaPsi { get; set; }

        public decimal? ProfundidadOriginalMm { get; set; }

        public decimal? ProfundidadAlertaMm { get; set; }

        public decimal? ProfundidadMinimaMm { get; set; }

        public uint? VidaUtilEstimadaKm { get; set; }
    }
}
