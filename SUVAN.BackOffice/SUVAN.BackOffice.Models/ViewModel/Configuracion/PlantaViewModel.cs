using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
    /// <summary>
    /// ViewModel para el formulario de alta y edición de Planta.
    /// Contiene la lista de Regiones filtradas por la empresa del usuario
    /// para garantizar la validación jerárquica de seguridad.
    /// </summary>
    public class PlantaViewModel
    {
        /// <summary>
        /// Identificador único de la planta.
        /// Valor 0 indica que se trata de una nueva planta (insert).
        /// Valor mayor a 0 indica edición (update).
        /// </summary>
        public int IdPlanta { get; set; }
        /// <summary>
        /// Identificador de la región seleccionada.
        /// Este selector se deshabilita en modo edición para preservar la jerarquía.
        /// </summary>
        [Required(ErrorMessage = "La Región es requerida")]
        public int IdRegion { get; set; }
        /// <summary>
        /// Identificador de la empresa del usuario autenticado.
        /// Se asigna desde el claim del usuario y se envía como campo oculto.
        /// </summary>
        public int IdEmpresa { get; set; }
        /// <summary>
        /// Nombre descriptivo de la planta.
        /// </summary>
        [Required(ErrorMessage = "El Nombre de la Planta es requerido")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 45 caracteres")]
        public string? NombrePlanta { get; set; }
        /// <summary>
        /// Nombre de la librería asociada a la planta (campo opcional).
        /// </summary>
        [StringLength(50, ErrorMessage = "La Librería no debe superar los 50 caracteres")]
        public string? Libreria { get; set; }
        /// <summary>
        /// Estatus de la planta almacenado como bit en la base de datos (0 = inactivo, 1 = activo).
        /// </summary>
        public ulong Activo { get; set; }
        /// <summary>
        /// Propiedad auxiliar que convierte el campo <see cref="Activo"/> (ulong)
        /// a un valor booleano para el uso con el checkbox en la vista.
        /// </summary>
        public bool ActivoBool
        {
            get => Activo != 0;
            set => Activo = value ? (ulong)1 : (ulong)0;
        }
        /// <summary>
        /// Lista de regiones disponibles para el selector del formulario.
        /// Filtradas por la empresa del usuario autenticado (jerarquía empresa → región).
        /// </summary>
       public List<RegionItemViewModel> Regiones { get; set; } = new();
        /// <summary>
        /// Submodelo que representa un ítem del selector de Región.
        /// </summary>
        public class RegionItemViewModel
        {
            /// <summary>
            /// Identificador de la región.
            /// </summary>
            public int IdRegion { get; set; }
            /// <summary>
            /// Nombre visible de la región en el selector.
            /// </summary>
            public string? Nombre { get; set; }
        }
    }
}
