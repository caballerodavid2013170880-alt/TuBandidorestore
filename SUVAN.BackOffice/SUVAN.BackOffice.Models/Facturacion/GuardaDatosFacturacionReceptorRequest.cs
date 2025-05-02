using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Facturacion
{
    public class GuardaDatosFacturacionReceptorRequest
    {
        [Required(ErrorMessage = "Falta el parámetro NombreRazonSocial")]
        public string NombreRazonSocial { get; set; }

        [Required(ErrorMessage = "Falta el parámetro RFC")]
        public string RFC { get; set; }


        [Required(ErrorMessage = "Falta el parámetro CodigoPostal")]
        public string CodigoPostal { get; set; }

        [Required(ErrorMessage = "Falta el parámetro RegimenFiscalId")]
        public int RegimenFiscalId { get; set; }

        [Required(ErrorMessage = "Falta el parámetro UsoCFDIId")]
        public int UsoCFDIId { get; set; }
    }

    public class ObtenDatosFacturacionReceptorResponse: GuardaDatosFacturacionReceptorRequest
    {
        public int Iddatosfacturacionreceptor { get; set; }
        public string RegimenFiscalClave { get; set; }

        public string RegimenFiscalDescripcion { get; set; }

        public string UsoCFDIClave { get; set; }

        public string UsoCFDIDescripcion { get; set; }
    }

}
