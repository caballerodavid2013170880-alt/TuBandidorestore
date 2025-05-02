using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.RegistroUsuario
{
    public class RegistroUsuarioRequest
    {
        [Required(ErrorMessage = "Falta el parámetro email")]
        //[JsonPropertyName("sampleValue")]

        public string? email { get; set; }

        [Required(ErrorMessage = "Falta el parámetro password")]
        public string? password { get; set; }

        [Required(ErrorMessage = "Falta el parámetro nombre")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "Falta el parámetro codigopais")]
        public int? codigopais { get; set; }

        [Required(ErrorMessage = "Falta el parámetro teléfono")]
        public string? telefono { get; set; }



    }
}
