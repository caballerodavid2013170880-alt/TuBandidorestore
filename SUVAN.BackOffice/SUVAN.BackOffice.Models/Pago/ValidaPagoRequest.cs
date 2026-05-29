using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Pago
{
    public class ValidaPagoRequest
    {
        [Required(ErrorMessage = "Falta el parámetro OrdenId")]

        public string OrdenId { get; set; }


        [Required(ErrorMessage = "Falta el parámetro PeticionId")]

        public string PeticionId { get; set; }
    }
}
