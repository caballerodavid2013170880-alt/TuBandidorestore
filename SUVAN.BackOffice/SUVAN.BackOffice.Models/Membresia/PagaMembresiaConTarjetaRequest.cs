using SUVAN.BackOffice.Models.Pago;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Membresia
{
    public class PagaMembresiaConTarjetaRequest :PagoParentRequest
    {

    }

    public class PagaMembresiaConMonederoRequest : PagoParentRequest
    {
          public string opt { get; set; }

    }

    public class PagaMembresiaConPayPalRequest : PagoParentRequest
    {

    }
}
