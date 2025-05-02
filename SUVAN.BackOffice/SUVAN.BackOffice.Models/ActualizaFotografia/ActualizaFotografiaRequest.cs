using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ActualizaFotografia
{
    public class ActualizaFotografiaRequest
    {
        [Required(ErrorMessage = "Falta el parámetro Imágen")]

        public string Imagen { get; set; }
    }
}
