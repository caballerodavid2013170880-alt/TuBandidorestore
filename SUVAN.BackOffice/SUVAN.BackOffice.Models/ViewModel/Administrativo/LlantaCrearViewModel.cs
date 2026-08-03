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

        public class ModeloDetalleViewModel
        {
            public int IdModeloLlanta { get; set; }
            public string? Marca { get; set; }
            public string? Modelo { get; set; }
            public string? Medida { get; set; }
            public decimal? PresionMinimaPsi { get; set; }
            public decimal? PresionMaximaPsi { get; set; }
            public decimal? ProfundidadOriginalMm { get; set; }
            public int? VidaUtilEstimadaKm { get; set; }
        }

        public ulong IdLlanta { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código de llanta es obligatorio")]
        [StringLength(30, ErrorMessage = "El código de llanta no debe exceder 30 caracteres")]
        public string CodigoLlanta { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de serie / DOT es obligatorio")]
        [StringLength(30, ErrorMessage = "El número de serie / DOT no debe exceder 30 caracteres")]
        public string NumeroSerieDot { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una marca")]
        public int IdMarcaLlanta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un modelo")]
        public int IdModeloLlanta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un estado")]
        public int IdEstadoLlanta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una región")]
        public int IdRegion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una planta")]
        public int IdPlanta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una zona")]
        public int IdZona { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un depósito")]
        public int IdDeposito { get; set; }

        public DateTime? FechaFabricacion { get; set; }

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

        [ValidateNever]
        public List<CatalogItemViewModel> Marcas { get; set; } = new();

        [ValidateNever]
        public List<CatalogItemViewModel> Modelos { get; set; } = new();

        [ValidateNever]
        public List<CatalogItemViewModel> EstadosLlanta { get; set; } = new();

        [ValidateNever]
        public ModeloDetalleViewModel? ModeloDetalle { get; set; }
    }
}
