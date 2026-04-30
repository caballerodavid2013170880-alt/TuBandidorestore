using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
    public class DeptoViewModel
    {
        [Required(ErrorMessage = "La Región es requerida")]
        public short id_region { get; set; }

        [Required(ErrorMessage = "La Planta es requerida")]
        public short id_planta { get; set; }

        [Required(ErrorMessage = "La Zona es requerida")]
        public short id_zona { get; set; }

        [Required(ErrorMessage = "El Depósito es requerido")]
        public short id_deposi { get; set; }

        public short id_depto { get; set; }

        [Required(ErrorMessage = "La Descripción es requerida")]
        [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres")]
        public string descrip { get; set; }

        [Required(ErrorMessage = "El Responsable es requerido")]
        [StringLength(100, ErrorMessage = "El responsable no puede exceder los 100 caracteres")]
        public string responsable { get; set; }

        // Propiedades de navegación para la vista (opcionales)
        public string? nombre_region { get; set; }
        public string? nombre_planta { get; set; }
        public string? nombre_zona { get; set; }
        public string? nombre_deposi { get; set; }
    }
}