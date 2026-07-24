using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class LlantaCrearViewModel
    {
        public class CatalogItemViewModel
        {
            public int Id { get; set; }
            public string? Nombre { get; set; }
        }

        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código de llanta es obligatorio")]
        [StringLength(30, ErrorMessage = "El código de llanta no debe exceder 30 caracteres")]
        public string CodigoLlanta { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de serie / DOT es obligatorio")]
        [StringLength(30, ErrorMessage = "El número de serie / DOT no debe exceder 30 caracteres")]
        public string NumeroSerieDot { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una región")]
        public int IdRegion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una planta")]
        public int IdPlanta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una zona")]
        public int IdZona { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un depósito")]
        public int IdDeposito { get; set; }

        [Range(0, 999.99, ErrorMessage = "La presión mínima no puede ser negativa")]
        public decimal? PresionMinimaPsi { get; set; }

        [Range(0, 999.99, ErrorMessage = "La presión máxima no puede ser negativa")]
        public decimal? PresionMaximaPsi { get; set; }

        public DateTime? FechaFabricacion { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La profundidad original no puede ser negativa")]
        public decimal? ProfundidadOriginalMm { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La vida útil estimada no puede ser negativa")]
        public int? VidaUtilEstimadaKm { get; set; }

        [Required(ErrorMessage = "El costo de adquisición es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El costo de adquisición no puede ser negativo")]
        public decimal? CostoAdquisicion { get; set; }

        [Required(ErrorMessage = "La fecha de adquisición es obligatoria")]
        public DateTime? FechaAdquisicion { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no deben exceder 500 caracteres")]
        public string? Observaciones { get; set; }

        [ValidateNever]
        public List<CatalogItemViewModel> Regiones { get; set; } = new();

        [ValidateNever]
        public List<CatalogItemViewModel> Plantas { get; set; } = new();

        [ValidateNever]
        public List<CatalogItemViewModel> Zonas { get; set; } = new();

        [ValidateNever]
        public List<CatalogItemViewModel> Depositos { get; set; } = new();
    }
}
