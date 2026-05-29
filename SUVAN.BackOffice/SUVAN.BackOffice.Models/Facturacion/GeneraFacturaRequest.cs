using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Facturacion
{

    public class GeneraFacturaRequest
    {
        [Required(ErrorMessage = "Falta el parámetro viaje_id")]
        public int viaje_id { get; set; }

        //[Required(ErrorMessage = "Falta el parámetro perfil_facturacion_id")]
        public int perfil_facturacion_id { get; set; }


    }
}
