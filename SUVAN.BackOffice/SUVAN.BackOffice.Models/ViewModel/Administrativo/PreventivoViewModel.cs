using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                // Solo fechas mayores o iguales al "hoy"
                return date.Date >= DateTime.Now.Date;
            }
            return true;
        }
    }


    public class PreventivoViewModel
    {
        public int Idpreventivo { get; set; }
        public int Idempresa { get; set; }

        [Required(ErrorMessage = "El Nombre del Preventivo es requerido")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 70 caracteres")]
        public string? NombrePreventivo { get; set; }

        [Required(ErrorMessage = "La Fecha Programada es requerida")]
        [FutureDate(ErrorMessage = "La fecha no puede ser anterior al día de hoy")]
        public DateTime? FechaPrev { get; set; }
        public string? ObservacionesPreventivo { get; set; }

        [Required(ErrorMessage = "La Región es requerida")]
        public int IdRegion { get; set; }

        [Required(ErrorMessage = "La Planta es requerida")]
        public int IdPlanta { get; set; }

        [Required(ErrorMessage = "La Zona es requerida")]
        public int IdZona { get; set; }

        [Required(ErrorMessage = "El Depósito es requerido")]
        public int IdDeposito { get; set; }

        [Required(ErrorMessage = "La Marca es requerida")]
        public short? IdMarca { get; set; }

        [Required(ErrorMessage = "El Modelo es requerido")]
        public int IdModelo { get; set; }

        [Required(ErrorMessage = "El Servicio es requerido")]
        public int IdManoObra { get; set; }

        // Catálogos
        public List<CatalogItemViewModel> Regiones { get; set; } = new();
        public List<CatalogItemViewModel> Plantas { get; set; } = new();
        public List<CatalogItemViewModel> Zonas { get; set; } = new();
        public List<CatalogItemViewModel> Depositos { get; set; } = new();
        public List<CatalogItemViewModel> Marcas { get; set; } = new();
        public List<CatalogItemViewModel> Modelos { get; set; } = new();
        public List<CatalogItemViewModel> ManosObra { get; set; } = new();

        public class CatalogItemViewModel
        {
            public int Id { get; set; }
            public string? Nombre { get; set; }
        }
    }

    public class DetalleGeneralViewModel
    {
        public int IdPreventivo { get; set; }
        public string? NombrePreventivo { get; set; }
        public string? Region { get; set; }
        public string? Planta { get; set; }
        public string? Zona { get; set; }   // Agregado
        public string? Deposito { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public DateTime FechaPrev { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal IvaUnitario { get; set; }
        public decimal IvaTotal { get; set; }
        public decimal CostoTotal { get; set; }
    }

    public class DetPrevMoItemViewModel
    {
        public int IdPrevMo { get; set; }
        public int IdPreventivo { get; set; }
        public string? NombrePreventivo { get; set; }
        public string? ManoObra { get; set; }
        public string? Placas { get; set; }
        public string? Vin { get; set; }
        public decimal? Iva { get; set; }
        public decimal CostoTotalUnitario { get; set; }
        public DateTime FechaPrev { get; set; }

        public List<string> Actividades { get; set; } = new List<string>();


        // Propiedades adicionales para filtros en datatables
        public string? Region { get; set; }
        public string? Planta { get; set; }
        public string? Zona { get; set; }
        public string? Deposito { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
    }
}