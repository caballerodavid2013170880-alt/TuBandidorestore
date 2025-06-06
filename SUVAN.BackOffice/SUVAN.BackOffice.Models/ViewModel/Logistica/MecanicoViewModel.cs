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
        public List<DepositosViewModel> DepositoView { get; set; } = new();

        public List<TallerViewModel> TallerView { get; set; } = new();

        public string DepositoJson { get; set; } = string.Empty;

        public string NombreDeposito { get; set; } = null!;

        public string NombreTaller { get; set; } = null!;

        public int IdMecanico { get; set; }

        public string Nombre { get; set; } = null!;

        public string Puesto { get; set; } = null!;

        public bool Activo { get; set; }

        public int IdDeposito { get; set; }

        public int IdTaller { get; set; }

        public class DepositosViewModel
        {
            public int DepositoId { get; set; }

            public string NombreDeposito { get; set; } = null!;

            public string DepositoNombreId => $"{DepositoId} - {NombreDeposito}";

            public List<TallerViewModel> Talleres { get; set; } = new();
        }

        public class TallerViewModel
        {
            public int IdTaller { get; set; }

            public string NombreTaller { get; set; } = null!;

            public string TallerNombreId => $"{IdTaller} - {NombreTaller}";
        }
    }
}
