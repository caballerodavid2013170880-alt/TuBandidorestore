using System;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class LlantaMarcaViewModel
    {
        public uint IdMarcaLlanta { get; set; }

        [Required(ErrorMessage = "La marca es requerida")]
        [StringLength(100, ErrorMessage = "La marca no debe exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
