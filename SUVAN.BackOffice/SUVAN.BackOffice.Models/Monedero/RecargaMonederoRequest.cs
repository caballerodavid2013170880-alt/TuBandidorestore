using SUVAN.BackOffice.Models.Pago;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Monedero
{
    public class RecargaMonederoRequest : PagoParentRequest
    {
        [Required(ErrorMessage = "Falta el parámetro Cantidad")]
        public decimal Cantidad { get; set; }
    }
}
