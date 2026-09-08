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
        public List<ZonasViewModel> ZonaView { get; set; } = new();

        public List<DepositosViewModel> DepositoView { get; set; } = new();

        public string ZonaJson { get; set; } = string.Empty;

        public int IdTaller { get; set; }
        public string NombreZona { get; set; } = string.Empty;

        public string NombreTaller { get; set; } = null!;

        public string NombreDeposito { get; set; } = string.Empty;

        public string? TTaller { get; set; }

        public ulong? Central { get; set; }

        public string Domicilio { get; set; } = null!;

        public string Contacto { get; set; } = null!;

        public string Telefono { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int? Iva { get; set; }

        public ulong? Refaccion { get; set; }

        public float? ValorUnitario { get; set; }

        //propiedades jerarquia cascada
        public int IdRegion { get; set; }
        public int IdPlanta { get; set; }
        public int IdZona { get; set; }
        public int IdDeposito { get; set; }

        //listas desplegables para caraga de datos
        //public List<CatalogItemViewModel> Regiones { get; set; } = new();
        public List<VehiculoDetalleViewModel.CatalogItemViewModel> Regiones { get; set; }
        public List<CatalogItemViewModel> Plantas { get; set; } = new();
        public List<CatalogItemViewModel> Zonas { get; set; } = new();
        public List<CatalogItemViewModel> Depositos { get; set; } = new();

        public class ZonasViewModel
        {
            public int ZonaId { get; set; }
            public string ZonaNombre { get; set; } = null!;
            public string IdNombre => $"{ZonaId} - {ZonaNombre}";
            public List<DepositosViewModel> Depositos { get; set; } = new();

        }

        public class DepositosViewModel
        {
            public int DepositoId { get; set; }

            public string NombreDeposito { get; set; } = null!;

            public string DepositoNombreId => $"{DepositoId} - {NombreDeposito}";
        }
    }
}
