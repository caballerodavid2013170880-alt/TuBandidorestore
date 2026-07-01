using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class TipoSiniestroViewModel
    {
        [Key]
        public int Id_tipo_siniestro { get; set; }

        /// <summary>
        /// Relación con la causa del siniestro.
        /// </summary>
        [Required]
        public int Id_causa_siniestro { get; set; }

        /// <summary>jhv
        /// Descripción del tipo de siniestro.
        /// </summary>
        [Required]
        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
