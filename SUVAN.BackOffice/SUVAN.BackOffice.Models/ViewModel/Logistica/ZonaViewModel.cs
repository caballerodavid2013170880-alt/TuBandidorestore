using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.DepositosDisponiblesViewModel;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class ZonaViewModel
    {
        public List<EmpresaViewModel> EmpresaView { get; set; } = new();
        public int ZonaId { get; set; }

        public string ZonaNombre { get; set; } = null!;

        public string Rfc { get; set; } = null!;

        public string Domicilio { get; set; } = null!;

        public string Telefono1 { get; set; } = null!;

        public string Telefono2 { get; set; } = null!;

        public string Responsable { get; set; } = null!;

        [DataType(DataType.Text)]
        public DateTime FechaApertura { get; set; }

        public int IdEmpresa { get; set; }

        public ulong Activo { get; set; }

        public List<DepositosViewModel> Depositos { get; set; } = new();

        public bool ActivoBool
        {
            get => Activo != 0;
            set => Activo = value ? (ulong)1 : (ulong)0;
        }
        public string IdNombre => $"{ZonaId} - {ZonaNombre}";

        public class EmpresaViewModel
        {
            public int EmpresaId { get; set; }
            public string? NombreCorto { get; set; }
            public string IdNombre => $"{EmpresaId} - {NombreCorto}";
        }

        public class DepositosViewModel
        {
            public int DepositoId { get; set; }

            public string NombreDeposito { get; set; } = null!;

            public string DepositoNombreId => $"{DepositoId} - {NombreDeposito}";
        }
    }
}
