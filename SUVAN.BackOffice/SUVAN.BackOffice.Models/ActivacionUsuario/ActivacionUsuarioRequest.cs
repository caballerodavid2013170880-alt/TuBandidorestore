using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ActivacionUsuario
{

    public class SolicitaActivacionRequest
    {

        [Required(ErrorMessage = "Falta el parámetro email")]
        public string email { get; set; }
    }

    public class ActivacionUsuarioRequest
    {
        [Required(ErrorMessage = "Falta el parámetro código")]

        public string codigo { get; set; }

        [Required(ErrorMessage = "Falta el parámetro email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Falta el parámetro password")]
        public string password { get; set; }
    }
}
