using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Membresia
{
    public class ConfirmaPagoMembresiaConTarjetaResponse
    {
        public string? Estatus { get; set; }
        public string? Vigencia { get; set; }
    }

    public class PagaMembresiaConMonederoResponse : ConfirmaPagoMembresiaConTarjetaResponse
    {
        public string? saldomonederoactualizado { get; set; }

    }

    public class ConfirmaPagoMembresiaConPayPalResponse : ConfirmaPagoMembresiaConTarjetaResponse
    {

    }

}
