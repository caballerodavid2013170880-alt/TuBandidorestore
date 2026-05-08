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
        public int id_region { get; set; }
        [Required(ErrorMessage = "La planta es obligatoria")]
        [Range(1, double.MaxValue, ErrorMessage = "Debes seleccionar una planta")]
        public int id_planta { get; set; }
        [Required(ErrorMessage = "La zona es obligatoria")]
        [Range(1, double.MaxValue, ErrorMessage = "Debes selecciona una zona")]
        public int id_zona { get; set; }


        public int id_deposi { get; set; }
        //public string nombre { get; set; } //no se agrego en la bd

        [Required(ErrorMessage = "El Nombre es requerido")]
        public string descrip { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        public string direc { get; set; }

        [Required(ErrorMessage = "La ciudad es requerida")]
        public string ciudad { get; set; }
        public string respon { get; set; }

        [Required(ErrorMessage = "El Teléfono es requerido")]
        public string tel { get; set; }
        public string loc_for { get; set; }
        public string r_person { get; set; }
        public short id_empresa { get; set; }
        public string desc_corta { get; set; }

        [Required(ErrorMessage = "El RFC es requerido")]
        public string rfc { get; set; }

        [Required(ErrorMessage = "El Código Postal es requerido")]
        public string cp { get; set; }

        [ValidateNever]
        public List<RegionModel> ListadoRegiones { get; set; }
        [ValidateNever]
        public List<RegionModel> ListadoPlantas { get; set; }
        [ValidateNever]
        public List<RegionModel> ListadoZonas { get; set; }


    }

}
