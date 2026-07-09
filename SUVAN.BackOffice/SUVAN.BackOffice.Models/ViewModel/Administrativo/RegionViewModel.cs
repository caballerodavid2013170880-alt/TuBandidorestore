using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class RegionViewModel
    {
        /// <summary>
        /// Identificador único de la región.
        /// Valor <c>0</c> indica nueva región (INSERT).
        /// Valor mayor a <c>0</c> indica edición (UPDATE).
        /// </summary>
        public int IdRegion { get; set; }
        /// <summary>
        /// Identificador de la empresa a la que pertenece la región.
        /// Se asigna desde el claim <c>User.GetEmpresaId()</c> y se envía
        /// como campo oculto en el formulario.
        /// En modo edición el campo se deshabilita en la vista para preservar la jerarquía.
        /// </summary>
        [Required(ErrorMessage = "La Empresa es requerida")]
        public int IdEmpresa { get; set; }
        /// <summary>
        /// Nombre descriptivo de la región.
        /// Debe ser único dentro de la misma empresa.
        /// </summary>
        [Required(ErrorMessage = "El Nombre de la Región es requerido")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 250 caracteres")]
        public string? NombreRegion { get; set; }
        /// <summary>
        /// Estatus de la región almacenado como bit en la base de datos
        /// (<c>0</c> = inactiva, <c>1</c> = activa).
        /// </summary>
        public ulong Activo { get; set; }
        /// <summary>
        /// Propiedad auxiliar que convierte <see cref="Activo"/> (ulong) a un valor
        /// booleano para poder usar el checkbox HTML en la vista Razor.
        /// </summary>
        public bool ActivoBool
        {
            get => Activo != 0;
            set => Activo = value ? (ulong)1 : (ulong)0;
        }
  }
}
