using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
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
        public int ZonaId { get; set; }
        public int IdEmpresa { get; set; }

        [Required(ErrorMessage = "La Región es obligatoria")]
        public int IdRegion { get; set; }

        [Required(ErrorMessage = "La Planta es obligatoria")]
        public int IdPlanta { get; set; }

        [Required(ErrorMessage = "El Nombre de la Zona es obligatoria")]
        public string ZonaNombre { get; set; } = null!;

        [Required(ErrorMessage = "El RFC es obligatorio")]
        public string Rfc { get; set; } = null!;

        public string Domicilio { get; set; } = null!;

        public string Telefono1 { get; set; } = null!;

        public string Telefono2 { get; set; } = null!;

        public string Responsable { get; set; } = null!;

        public ulong Activo { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaApertura { get; set; } = DateTime.Now;

        [ValidateNever]
        public List<CatalogItemViewModel> Regiones { get; set; } = new();
        [ValidateNever]
        public List<CatalogItemViewModel> Plantas { get; set; } = new();


        public bool ActivoBool
        {
            get => Activo != 0;
            set => Activo = value ? (ulong)1 : (ulong)0;
        }
        public string IdNombre => $"{ZonaId} - {ZonaNombre}";

        public class CatalogItemViewModel
        {
            public int Id { get; set; }
            public string? Nombre { get; set; }
        }
    }

    //Esta clase solo se usa si se listan empresas dentro de esteviewmodel
    //public class EmpresaViewModel
    //{
    //    public int EmpresaId { get; set; }
    //    public string? NombreCorto { get; set; }
    //    public string IdNombre => $"{EmpresaId} - {NombreCorto}";
    //}

    //public class RegionModel
    //{
    //    public int Id { get; set; }
    //    public string Nombre { get; set; }
    //}



    //public class DepositosViewModel
    //{
    //    public int DepositoId { get; set; }

    //    public string NombreDeposito { get; set; } = null!;

    //    public string DepositoNombreId => $"{DepositoId} - {NombreDeposito}";
    //}
}

