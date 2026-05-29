using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
    //Clases "espejo" para evitar cargar las entidades de la bd aqui
    public class RegionModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class DepositoViewModel
    {
        [Required(ErrorMessage = "La región es obligatoria")]
        [Range (1, double.MaxValue,ErrorMessage ="Debes seleccionar una región")]
        public int IdRegion { get; set; }
        [Required(ErrorMessage = "La planta es obligatoria")]
        [Range(1, double.MaxValue, ErrorMessage = "Debes seleccionar una planta")]
        public int IdPlanta { get; set; }
        [Required(ErrorMessage = "La zona es obligatoria")]
        [Range(1, double.MaxValue, ErrorMessage = "Debes selecciona una zona")]
        public int IdZona { get; set; }


        public int IdDeposito { get; set; }
        //public string nombre { get; set; } //no se agrego en la bd

        [Required(ErrorMessage = "El Nombre es requerido")]
        public string NombreDeposito { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        public string Direc { get; set; }

        [Required(ErrorMessage = "La ciudad es requerida")]
        public string Ciudad { get; set; }
        public string Respon { get; set; }

        [Required(ErrorMessage = "El Teléfono es requerido")]
        public string Tel { get; set; }

        [Required(ErrorMessage = "El campo Local/Foráneo es obligatorio")]
        [StringLength(1,ErrorMessage = "Solo se permite 1 ´letra L o F")]
        [RegularExpression(@"^[LF]$|^[lf]$",ErrorMessage = "Solo se permite la letra L o F")]
        public string LocFor { get; set; }
        public string RPerson { get; set; }
        public int IdEmpresa { get; set; }
        public string DescCorta { get; set; }

        [Required(ErrorMessage = "El RFC es requerido")]
        public string Rfc { get; set; }

        [Required(ErrorMessage = "El Código Postal es requerido")]
        public string Cp { get; set; }
        public bool Activo { get; set; } = true;

        [ValidateNever]
        public List<RegionModel> ListadoRegiones { get; set; }
        [ValidateNever]
        public List<RegionModel> ListadoPlantas { get; set; }
        [ValidateNever]
        public List<RegionModel> ListadoZonas { get; set; }


    }

}
