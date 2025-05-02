using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.Conductor
{
    public class PerfilConductorModel
    {
        public int Idconductor { get; set; }
        public string? Nombre { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public int? Codigopais { get; set; }
        [Range(4, 4, ErrorMessage = "El codigo debe ser de 4 digitos")]
        public string CodigoCambioTelefono { get; set; } = "0000";
        [Required]
        public string? Telefono { get; set; }
        public string? FotoConductor { get; set; }

    }
}
