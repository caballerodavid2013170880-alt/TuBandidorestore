using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class CausaSiniestroViewModel
    {
        [Key]
        public int Id_causa_siniestro { get; set; }

        /// <summary>
        /// Descripción de la causa del siniestro.
        /// </summary>
        [Required]
        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        public class TipoSiniestroViewModel
        {
        }
    }
}