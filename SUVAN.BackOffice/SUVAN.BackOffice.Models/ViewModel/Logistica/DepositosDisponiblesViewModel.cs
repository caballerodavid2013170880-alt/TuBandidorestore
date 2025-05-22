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

        public int ZonaId { get; set; }

        public string NombreDeposito { get; set; } = null!;

        public int TallerId { get; set; }

        public bool Activo { get; set; } = true;

        public string ZonaJson { get; set; } = string.Empty;

        public List<ZonasViewModel> ZonasView { get; set; } = new();

        public List<TalleresViewModel> TalleresView { get; set; } = new();

        public string DepsoitoIdNombre => $"{DepositoId} - {NombreDeposito}";

        public class ZonasViewModel
        {
            public int ZonaId { get; set; }
            public string ZonaNombre { get; set; } = null!;
            public string IdNombre => $"{ZonaId} - {ZonaNombre}";
            public List<TalleresViewModel> Talleres { get; set; } = new();

        }

        public class TalleresViewModel
        {
            public int IdTaller { get; set; }

            public string NombreTaller { get; set; } = null!;

            public string TallerNombreId => $"{IdTaller} - {NombreTaller}";
        }

    }
}
