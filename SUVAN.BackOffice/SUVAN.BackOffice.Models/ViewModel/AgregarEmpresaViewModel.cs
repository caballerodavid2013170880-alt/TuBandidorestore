using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
    public class AgregarEmpresaViewModel
    {

        public List<TipoRegimenFiscalModel> TipoRegimen { get; set; }
        public int EmpresaId { get; set; }

        [Required(ErrorMessage = "El Nombre es requerido")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El NombreCorto es requerido")]
        public string NombreCorto { get; set; } = null!;

        [Required(ErrorMessage = "El RFC es requerido")]
        public string Rfc { get; set; } = null!;

        [Required(ErrorMessage = "El Régimen Fiscal es requerido")]
        public int? idRegimenFiscal { get; set; }

        [Required(ErrorMessage = "La Serie Fiscal es requerida")]
        public string Serie { get; set; }

        [Required(ErrorMessage = "El Folio Fiscal es requerido")]
        public int Folio { get; set; }

        [Required(ErrorMessage = "El Código Postal es requerido")]
        public string CP { get; set; }

        public bool Activo { get; set; } = true;
        public AgregarEmpresaViewModel()
        {

            TipoRegimen = new List<TipoRegimenFiscalModel>();

        }
    }
}
