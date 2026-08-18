using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class LlantaModeloViewModel
    {
        public uint IdModeloLlanta { get; set; }

        [Required(ErrorMessage = "La marca es requerida")]
        [Range(1, uint.MaxValue, ErrorMessage = "La marca es requerida")]
        public uint IdMarcaLlanta { get; set; }

        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es requerido")]
        [StringLength(150, ErrorMessage = "El modelo no debe exceder 150 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "La medida no debe exceder 50 caracteres")]
        public string? Medida { get; set; }

        [Range(0, 999.99, ErrorMessage = "La presión mínima debe ser mayor o igual a cero")]
        public decimal? PresionMinimaPsi { get; set; }

        [Range(0, 999.99, ErrorMessage = "La presión máxima debe ser mayor o igual a cero")]
        public decimal? PresionMaximaPsi { get; set; }

        [Range(0, 999.99, ErrorMessage = "La profundidad original debe ser mayor o igual a cero")]
        public decimal? ProfundidadOriginalMm { get; set; }

        public uint? VidaUtilEstimadaKm { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public List<CatalogItemViewModel> Marcas { get; set; } = new();

        public class CatalogItemViewModel
        {
            public uint Id { get; set; }

            public string Nombre { get; set; } = string.Empty;
        }
    }
}
