using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Conductor
{
    public class ConductorRegistroRequest
    {
        [Required(ErrorMessage = "Falta el parámetro email")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Falta el parámetro empresa")]
        public int? empresa { get; set; }

        [Required(ErrorMessage = "Falta el parámetro nombre")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "Falta el parámetro codigopais")]
        public int? codigopais { get; set; }

        [Required(ErrorMessage = "Falta el parámetro teléfono")]
        public string? telefono { get; set; }

        [Required(ErrorMessage = "Falta el parámetro RFC")]
        public string? rfc { get; set; }

    }
}
