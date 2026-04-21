using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
    public class RegionViewModel
    {
        public short id_empresa { get; set; }
        public short id_region { get; set; }
        [Required(ErrorMessage = "El Nombre es requerido")]
        public string nombre { get; set; }
  }
}
