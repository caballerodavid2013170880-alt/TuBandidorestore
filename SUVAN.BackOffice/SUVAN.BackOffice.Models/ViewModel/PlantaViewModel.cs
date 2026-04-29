using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
    public class PlantaViewModel
    {
        public int id_emp { get; set; }

        [Required(ErrorMessage = "La Región es requerida")]
        public short id_region { get; set; }

        public short id_planta { get; set; }

        [Required(ErrorMessage = "El Nombre es requerido")]
        [StringLength(45, ErrorMessage = "El nombre no puede exceder los 45 caracteres")]
        public string nombre { get; set; }

        [StringLength(50, ErrorMessage = "La librería no puede exceder los 50 caracteres")]
        public string libreria { get; set; }

        public string? nombre_region { get; set; }
    }
}
