using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class ManoObraViewModel
    {
        public int IdManoObra { get; set; }
        [Required(ErrorMessage = "La descripción es requerida")]
        [Display(Name = "Descripción del Servicio")]
        public string? DescripcionManoobra { get; set; }
        [Required(ErrorMessage = "El costo unitario es requerido")]
        [Display(Name = "Costo Unitario")]
        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser mayor o igual a 0")]
        public decimal? CostoUnitario { get; set; }
        public List<ManoObraDetalleViewModel> Detalles { get; set; } = new List<ManoObraDetalleViewModel>();
    }
    public class ManoObraDetalleViewModel
    {
        public int IdMoDetalle { get; set; }
        public int IdManoObra { get; set; }
        [Required(ErrorMessage = "La descripción de la actividad es requerida")]
        public string DescripcionActividad { get; set; } = string.Empty;
        public bool EsObligatorio { get; set; }

        public string? ServicioPadre { get; set; } // Para la vista consolidada de detalles
    }
}