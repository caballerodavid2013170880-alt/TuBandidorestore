using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Auth.User
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Falta el parámetro UserName")]

        public string UserName { get; set; }

        [Required(ErrorMessage = "Falta el parámetro Password")]

        public string Password { get; set; }
    }
}
