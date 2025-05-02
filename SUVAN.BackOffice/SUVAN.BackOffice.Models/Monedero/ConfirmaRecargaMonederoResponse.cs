using SUVAN.BackOffice.Models.Pago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Monedero
{
    public class ConfirmaRecargaMonederoResponse : ConfirmarPagoResponse
    {

        public decimal? Saldo { get; set; }
    }
}
