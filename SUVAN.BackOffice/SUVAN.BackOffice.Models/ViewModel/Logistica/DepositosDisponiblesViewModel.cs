using SUVAN.BackOffice.Models.Facturacion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class DepositosDisponiblesViewModel
    {
        [Required(ErrorMessage = "Seleccione una zona")]
        public List<ZonaViewModel> ZonaView { get; set; }

        public int DepositoId { get; set; }

        [Required(ErrorMessage = "La Zona es requerida")]
        public int ZonaId { get; set; }

        [Required(ErrorMessage = "El Nombre del Deposito es requerido")]
        public string NombreDeposito { get; set; } = null!;

        [Required(ErrorMessage = "El Taller es requerido")]
        public int TallerId { get; set; }

        public bool Activo { get; set; } = true;

        public DepositosDisponiblesViewModel()
        {

            ZonaView = new List<ZonaViewModel>();

        }
    }
}
