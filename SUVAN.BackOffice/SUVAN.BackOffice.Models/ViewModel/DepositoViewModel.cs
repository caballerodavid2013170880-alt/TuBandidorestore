using System;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel
{
    public class DepositoViewModel
    {
        public short? id_empresa { get; set; }

        [Required(ErrorMessage = "La Región es requerida")]
        public short id_region { get; set; }

        [Required(ErrorMessage = "La Planta es requerida")]
        public short id_planta { get; set; }

        [Required(ErrorMessage = "La Zona es requerida")]
        public short id_zona { get; set; }

        public short id_deposito { get; set; }

        [Required(ErrorMessage = "La Descripción es requerida")]
        [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres")]
        public string descripcion { get; set; } = string.Empty;

        public string? direccion { get; set; }
        public string? ciudad { get; set; }
        public string? responsable { get; set; }
        public string? telefono { get; set; }
        public string? loc_for { get; set; }
        public string? r_person { get; set; }
        public string? desc_corta { get; set; }
        public string? rfc { get; set; }
        public string? cp { get; set; }

        // Propiedades de navegación para UI
        public string? nombre_empresa { get; set; }
        public string? nombre_region { get; set; }
        public string? nombre_planta { get; set; }
        public string? nombre_zona { get; set; }
    }
}
