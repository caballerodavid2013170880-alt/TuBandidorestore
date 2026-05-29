using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ActualizaPassword
{

    public class ActualizaPasswordRequest
    {
        [Required(ErrorMessage = "Falta el parámetro email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Falta el parámetro nuevopassword")]
        public string nuevopassword { get; set; }

    }
}
