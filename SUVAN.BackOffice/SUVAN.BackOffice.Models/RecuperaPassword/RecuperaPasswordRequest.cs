using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.RecuperaPassword
{
    public class RecuperaPasswordRequest
    {
        [Required(ErrorMessage = "Falta el parámetro email")]
        public string email { get; set; }

    }
}
