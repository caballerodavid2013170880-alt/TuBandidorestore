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
        public int DepositoId { get; set; }

        [Required(ErrorMessage = "La Zona Id es reuqerida")]
        public int ZonaId { get; set; }

        [Required(ErrorMessage = "El Nombre de la Zona es requerido")]
        public string ZonaNombre { get; set; } = null!;

        [Required(ErrorMessage = "El Nombre del Deposito es requerido")]
        public string NombreDeposito { get; set; } = null!;

        [Required(ErrorMessage = "El Taller Id es reuqerido")]
        public int TallerId { get; set; }

        [Required(ErrorMessage = "El Nombre del Taller es requerido")]
        public string NombreTaller { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }
}
