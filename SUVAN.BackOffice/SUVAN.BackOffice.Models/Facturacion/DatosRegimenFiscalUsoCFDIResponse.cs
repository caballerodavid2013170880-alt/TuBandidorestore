using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Facturacion
{
    public class DatosRegimenFiscalUsoCFDIResponse
    {
        public int id { get; set; }
        public string? clave { get; set; }
        public string? descripcion { get; set; }

        public class RegimenFiscalReceptorResponse : DatosRegimenFiscalUsoCFDIResponse
        {
        }

        public class UsoCFDIReceptorResponse : DatosRegimenFiscalUsoCFDIResponse
        {

        }
    }

    public class TipoRegimenFiscalModel
    {
        public int idRegimenFiscal { get; set; }
        public string? clave { get; set; }
        public string? descripcion { get; set; }

        // Propiedad combinada para mostrar en el dropdown
        public string ClaveDescripcion => $"{clave} - {descripcion}";
    }
}
