using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    //Clases "espejo" para evitar cargar las entidades de la bd aqui

        public class DepositoViewModel
    {
        public class CatalogItemViewModel
        {
            public int Id { get; set; }
            public string? Nombre { get; set; }
        }

        [Required(ErrorMessage = "La Región es obligatoria")]
        [Range (1, double.MaxValue,ErrorMessage ="Debes seleccionar una región")]
        public int IdRegion { get; set; }
        [Required(ErrorMessage = "La Planta es obligatoria")]
        [Range(1, double.MaxValue, ErrorMessage = "Debes seleccionar una planta")]
        public int IdPlanta { get; set; }
        [Required(ErrorMessage = "La Zona es obligatoria")]
        [Range(1, double.MaxValue, ErrorMessage = "Debes selecciona una zona")]
        public int IdZona { get; set; }


        public int IdDeposito { get; set; }
        //public string nombre { get; set; } //no se agrego en la bd

        [Required(ErrorMessage = "El Nombre del Depósito es requerido")]
        [StringLength(80, MinimumLength = 10, ErrorMessage = "El nombre debe tener entre 10 y 80 caracteres")]
        public string NombreDeposito { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(250, MinimumLength = 10, ErrorMessage = "La dirección debe tener entre 10 y 250 caracteres")]
        public string Direc { get; set; }

        [Required(ErrorMessage = "La ciudad es requerida")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "La ciudad debe tener entre 3 y 50 caracteres")]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "El Responsable es requerido")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "El responsable debe tener entre 4 y 100 caracteres")]
        public string Respon { get; set; }

        [Required(ErrorMessage = "El Teléfono es requerido")]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "El teléfono debe tener entre 10 y 12 caracteres")]
        public string Tel { get; set; }

        //validar que solo se permita L o F y que sea obligatorio
        [Required(ErrorMessage = "El campo Local/Foráneo es obligatorio")]
        [StringLength(1,ErrorMessage = "Solo se permite 1 ´letra L o F")]
        [RegularExpression(@"^[LF]$|^[lf]$",ErrorMessage = "Solo se permite la letra L o F")]
        public string LocFor { get; set; }
        public string RPerson { get; set; }
        public int IdEmpresa { get; set; }
        public string NomCorto { get; set; }

        [Required(ErrorMessage = "El RFC es requerido")]
        [RegularExpression(@"^([A-Z&Ññ]{3,4})(\d{6})([A-V1-9])([A-Z\d]{1,4})$", ErrorMessage = "El RFC no es válido")]
        public string Rfc { get; set; }

        [Required(ErrorMessage = "El Código Postal es requerido")]
        [RegularExpression(@"^\d{4,5}$", ErrorMessage = "El Código Postal no es válido")]
        public string Cp { get; set; }
        public ulong Activo { get; set; }
        public bool ActivoBool
        {
            get => Activo != 0;
            set => Activo = value? (ulong)1 : (ulong)0;
        }

        [ValidateNever]
        public List<CatalogItemViewModel> Regiones { get; set; } = new();
        [ValidateNever]
        public List<CatalogItemViewModel> Plantas { get; set; } = new();
        [ValidateNever]
        public List<CatalogItemViewModel> Zonas { get; set; } = new();


    }

}
