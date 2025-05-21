using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class TallerViewModel
    {
        [Required(ErrorMessage = "Seleccione una zona")]
        public List<ZonaViewModel> ZonaView { get; set; }

        public int IdTaller { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Zona")]
        public int ZonaIdzona { get; set; }

        [Required(ErrorMessage = "El Nombre del taller es requerido")]
        public string NombreTaller { get; set; } = null!;

        public TallerViewModel()
        {

            ZonaView = new List<ZonaViewModel>();

        }
    }
}
