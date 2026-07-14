using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class DeptoViewModel
    {
        public class CatalogItemViewModel
        {
            public int Id { get; set; }
            public string? Nombre { get; set; }
        }
        /// <summary>
        /// Identificador único del departamento.
        /// Valor <c>0</c> indica alta (INSERT).
        /// Valor mayor a <c>0</c> indica edición (UPDATE).
        /// </summary>
        public int IdDepto { get; set; }
        /// <summary>
        /// Identificador de la empresa del usuario autenticado.
        /// Se asigna desde el claim <c>User.GetEmpresaId()</c>
        /// y se envía como campo oculto en el formulario.
        /// </summary>
        public int IdEmpresa { get; set; }
        /// <summary>
        /// Identificador de la región seleccionada.
        /// Selector deshabilitado en modo edición para preservar la jerarquía.
        /// Al Agregar actúa como disparador de la cascada Región → Planta.
        /// </summary>
        [Required(ErrorMessage = "La Región es requerida")]
        public int IdRegion { get; set; }
        /// <summary>
        /// Identificador de la planta seleccionada.
        /// Se carga en cascada al seleccionar la región (AJAX).
        /// Selector deshabilitado en modo edición.
        /// Al Agregar actúa como disparador de la cascada Planta → Zona.
        /// </summary>
        [Required(ErrorMessage = "La Planta es requerida")]
        public int IdPlanta { get; set; }
        /// <summary>
        /// Identificador de la zona seleccionada.
        /// Se carga en cascada al seleccionar la planta (AJAX).
        /// Selector deshabilitado en modo edición.
        /// Al Agregar actúa como disparador de la cascada Zona → Depósito.
        /// </summary>
        [Required(ErrorMessage = "La Zona es requerida")]
        public int IdZona { get; set; }
        /// <summary>
        /// Identificador del depósito seleccionado.
        /// Se carga en cascada al seleccionar la zona (AJAX).
        /// Selector deshabilitado en modo edición.
        /// </summary>
        [Required(ErrorMessage = "El Depósito es requerido")]
        public int IdDeposito { get; set; }
        /// <summary>
        /// Nombre descriptivo del departamento.
        /// Debe ser único dentro del mismo depósito y empresa.
        /// Máximo 70 caracteres.
        /// </summary>
        [Required(ErrorMessage = "El Nombre del Departamento es requerido")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 70 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-_.,áéíóúÁÉÍÓÚñÑ]*$", ErrorMessage = "Caracteres especiales no permitidos detectados.")]
        public string? NombreDepto { get; set; }
        /// <summary>
        /// Nombre del responsable del departamento. Máximo 70 caracteres.
        /// </summary>
        [Required(ErrorMessage = "El Responsable es requerido")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "El responsable debe tener entre 3 y 70 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-_.,áéíóúÁÉÍÓÚñÑ]*$", ErrorMessage = "Caracteres especiales no permitidos detectados.")]
        public string? Responsable { get; set; }
        /// <summary>
        /// Estatus del departamento almacenado como bit en la base de datos
        /// (<c>0</c> = inactivo, <c>1</c> = activo).
        /// </summary>
        public ulong Activo { get; set; }
        /// <summary>
        /// Propiedad auxiliar que convierte <see cref="Activo"/> (ulong) a un valor
        /// booleano para usar el checkbox HTML en la vista Razor.
        /// </summary>
        public bool ActivoBool
        {
            get => Activo != 0;
            set => Activo = value ? (ulong)1 : (ulong)0;
        }
        /// <summary>
        /// Lista de regiones disponibles para el selector del formulario.
        /// Filtradas por la empresa del usuario autenticado (jerarquía empresa → región).
        /// Se carga siempre desde el servidor en el GET de la vista.
        /// </summary>
        public List<CatalogItemViewModel> Regiones { get; set; } = new();
        /// <summary>
        /// Lista de plantas disponibles para el selector del formulario.
        /// Cargada en cascada vía AJAX al seleccionar la región en modo alta.
        /// Al Editar se pre-carga desde el servidor con las plantas de la región guardada.
        /// </summary>
        public List<CatalogItemViewModel> Plantas { get; set; } = new();
        /// <summary>
        /// Lista de zonas disponibles para el selector del formulario.
        /// Cargada en cascada vía AJAX al seleccionar la planta en modo alta.
        /// Al Editar se pre-carga desde el servidor con las zonas de la planta guardada.
        /// </summary>
        public List<CatalogItemViewModel> Zonas { get; set; } = new();
        /// <summary>
        /// Lista de depósitos disponibles para el selector del formulario.
        /// Cargada en cascada vía AJAX al seleccionar la zona en modo alta.
        /// Al Editar se pre-carga desde el servidor con los depósitos de la zona guardada.
        /// </summary>
        public List<CatalogItemViewModel> Depositos { get; set; } = new();
        /// <summary>
        /// JSON serializado con la jerarquía completa Región → Planta → Zona → Depósito
        /// para la carga en cascada de los selectores en el cliente.
        /// Solo se usa en modo alta; en modo edición es null.
        /// </summary>
        public string? CascadeJson { get; set; }
        // ──────────────────────────────────────────────────────────────────
        //  Submodelo de ítem para cada selector
        // ──────────────────────────────────────────────────────────────────
        
    }
}
