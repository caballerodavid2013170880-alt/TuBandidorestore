using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Pago
{
    public class PagoParentRequest
    {
        [Required(ErrorMessage = "Falta el parámetro TipoTransaccionId")]

        public int TipoTransaccionId { get; set; }

        [Required(ErrorMessage = "Falta el parámetro MetodoPagoId")]

        public int MetodoPagoId { get; set; }
    }
    
    public class PagoRequest : PagoParentRequest
    {

        [Required(ErrorMessage = "Falta el parámetro reservacionId")]

        public int reservacionId { get; set; }


        [Required(ErrorMessage = "Falta el parámetro pasajeros")]

        public int pasajeros { get; set; }


        public string codigodescuento { get; set; }

    }

    public class PagoMonederoRequest :PagoRequest
    {
        public string opt { get; set; }

    }

}
