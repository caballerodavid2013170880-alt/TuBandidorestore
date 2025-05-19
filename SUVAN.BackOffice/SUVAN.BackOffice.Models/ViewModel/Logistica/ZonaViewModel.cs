using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class ZonaViewModel
    {
        public int ZonaId { get; set; }

        [Required(ErrorMessage = "El Nombre de la Zona es requerida")]
        public string ZonaNombre { get; set; } = null!;

        // Propiedad combinada para mostrar en el dropdown
        public string IdNombre => $"{ZonaId} - {ZonaNombre}";
    }
}
