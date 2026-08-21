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
    }

    public class LlantaInspeccionCatalogoViewModel
    {
        public ulong Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }

    public class LlantaInspeccionGuardarViewModel
    {
        public int IdVehiculo { get; set; }

        public ulong IdTipoInspeccion { get; set; }

        public DateTime FechaInspeccion { get; set; }

        public uint KilometrajeLlanta { get; set; }

        public List<LlantaInspeccionDetalleGuardarViewModel> Detalles { get; set; } = new();
    }

    public class LlantaInspeccionDetalleGuardarViewModel
    {
        public ulong IdLlantaAsignacion { get; set; }

        public decimal? ProfundidadMm { get; set; }

        public decimal? PresionPsi { get; set; }

        public ushort IdEstadoInspeccion { get; set; }

        public ushort IdConclusionInspeccion { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no deben exceder 1000 caracteres")]
        public string? Observaciones { get; set; }
    }
}
