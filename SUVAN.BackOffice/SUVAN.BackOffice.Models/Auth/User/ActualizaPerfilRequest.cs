using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Auth.User
{
    public class ActualizaPerfilRequest 
    {
        public string? Nombre { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public int? Codigopais { get; set; }
        //[Range(4, 4, ErrorMessage = "El codigo debe ser de 4 dijitos")]
        public string CodigoCambioTelefono { get; set; } = "0000";
        [Required]
        public string? Telefono { get; set; }
    }
}
