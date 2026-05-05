using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
    /// <summary>
    /// ViewModel para la entidad Depto del módulo de Configuración.
    /// </summary>
    public class DeptoViewModel
    {
        /// <summary>
        /// Identificador de la Región asociada al departamento.
        /// </summary>
        [Required(ErrorMessage = "La Región es requerida")]
        public short id_region { get; set; }

        /// <summary>
        /// Identificador de la Planta asociada al departamento.
        /// </summary>
        [Required(ErrorMessage = "La Planta es requerida")]
        public short id_planta { get; set; }

        /// <summary>
        /// Identificador de la Zona asociada al departamento.
        /// </summary>
        [Required(ErrorMessage = "La Zona es requerida")]
        public short id_zona { get; set; }

        /// <summary>
        /// Identificador del Depósito asociado al departamento.
        /// </summary>
        [Required(ErrorMessage = "El Depósito es requerido")]
        public short id_deposi { get; set; }

        /// <summary>
        /// Identificador único del Departamento.
        /// </summary>
        public short id_depto { get; set; }

        /// <summary>
        /// Descripción o nombre del departamento.
        /// </summary>
        [Required(ErrorMessage = "La Descripción es requerida")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder los 255 caracteres")]
        public string descripcion { get; set; }

        /// <summary>
        /// Nombre del responsable del departamento.
        /// </summary>
        [Required(ErrorMessage = "El Responsable es requerido")]
        [StringLength(255, ErrorMessage = "El nombre del responsable no puede exceder los 255 caracteres")]
        public string responsable { get; set; }
    }
}
