using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class MecanicoViewModel
    {
        public List<DepositosDisponiblesViewModel> DepositoView { get; set; }

        public string NombreDeposito { get; set; } = null!;

        public int IdMecanico { get; set; }

        public string Nombre { get; set; } = null!;

        public string Apellido { get; set; } = null!;

        public string? Numero { get; set; }

        [DataType(DataType.Text)]
        public DateTime FechaIngreso { get; set; }

        public bool Activo { get; set; }

        public int IdDeposito { get; set; }

        public MecanicoViewModel()
        {

            DepositoView = new List<DepositosDisponiblesViewModel>();

        }
    }
}
