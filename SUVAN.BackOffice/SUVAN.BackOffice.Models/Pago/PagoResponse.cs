using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Pago
{
    public class PagoParentResponse
    {
        public string? url { get; set; }

        public string? OrdenId { get; set; }
        public string? PeticionId { get; set; }
    }

    public class PagoConTarjetaResponse : PagoParentResponse
    {

    }

    public class PagoResponse : PagoParentResponse
    {

    }

    public class PagoMonederoResponse
    {
        public string? saldomonederoactualizado { get; set; }

        public string? OrdenId { get; set; }
        public string? PeticionId { get; set; }
    }

}
