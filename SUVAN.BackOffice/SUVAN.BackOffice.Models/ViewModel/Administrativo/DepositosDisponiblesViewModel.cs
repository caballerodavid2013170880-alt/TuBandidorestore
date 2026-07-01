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

        public bool Activo { get; set; } = true;

        public string ZonaJson { get; set; } = string.Empty;

        public string Dirección { get; set; } = null!;

        public string Ciudad { get; set; } = null!;

        public string Responsable { get; set; } = null!;

        public string Teléfono { get; set; } = null!;

        public string? LocFor { get; set; }

        public string? RPerson { get; set; }

        public int IdEmpresa { get; set; }

        public string NombreCorto { get; set; } = null!;

        public string Rfc { get; set; } = null!;

        public string Cp { get; set; } = null!;

        public List<ZonasViewModel> ZonasView { get; set; } = new();

        public List<EmpresaViewModel> EmpresaView { get; set; } = new();

        public List<TallerViewModel> Talleres { get; set; } = new();

        public string DepsoitoIdNombre => $"{DepositoId} - {NombreDeposito}";

        public class ZonasViewModel
        {
            public int ZonaId { get; set; }
            public string ZonaNombre { get; set; } = null!;
            public string IdNombre => $"{ZonaId} - {ZonaNombre}";

        }

        public class EmpresaViewModel
        {
            public int IdEmpresa { get; set; }

            public string NombreCorto{ get; set; } = null!;

            public string TallerNombreId => $"{IdEmpresa} - {NombreCorto}";
        }

        public class TallerViewModel
        {
            public int IdTaller { get; set; }

            public string NombreTaller { get; set; } = null!;

            public string TallerNombreId => $"{IdTaller} - {NombreTaller}";
        }

    }
}
