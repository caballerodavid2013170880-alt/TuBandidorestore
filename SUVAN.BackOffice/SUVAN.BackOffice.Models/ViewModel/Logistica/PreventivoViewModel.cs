using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    /// <summary>
    /// Modelo de vista para la transferencia de datos de Mantenimiento Preventivo entre la UI y el Controlador.
    /// </summary>
    public class PreventivoViewModel
    {
        public int Idpreventivo { get; set; }
        public int Idempresa { get; set; }

        [Required(ErrorMessage = "El Nombre del Preventivo es requerido")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 70 caracteres")]
        public string? NombrePreventivo { get; set; }

        [Required(ErrorMessage = "La Fecha Programada es requerida")]
        public DateTime? FechaPrev { get; set; }

        public string? ObservacionesPreventivo { get; set; }

        [Required(ErrorMessage = "La Planta es requerida")]
        public int IdPlanta { get; set; }

        [Required(ErrorMessage = "El Depósito es requerido")]
        public int IdDeposito { get; set; }

        [Required(ErrorMessage = "La Marca es requerida")]
        public short? IdMarca { get; set; }

        [Required(ErrorMessage = "El Modelo es requerido")]
        public int IdModelo { get; set; }

        [Required(ErrorMessage = "La Mano de Obra es requerida")]
        public int IdManoObra { get; set; }

        public List<PlantaItemViewModel> Plantas { get; set; } = new();
        public List<DepositoItemViewModel> Depositos { get; set; } = new();
        public List<MarcaItemViewModel> Marcas { get; set; } = new();
        public List<ModeloItemViewModel> Modelos { get; set; } = new();
        public List<ManoObraItemViewModel> ManosObra { get; set; } = new();

        public class PlantaItemViewModel 
        { 
            public int IdPlanta { get; set; } 
            public string? Nombre { get; set; } 
        }
        public class DepositoItemViewModel 
        { 
            public int IdDeposito { get; set; } 
            public string? Nombre { get; set; } 
        }
        public class MarcaItemViewModel 
        {   public short IdMarca { get; set; } 
            public string? Nombre { get; set; } 
        }
        public class ModeloItemViewModel 
        {   public int IdModelo { get; set; } 
            public string? Nombre { get; set; } 
        }
        public class ManoObraItemViewModel 
        {
            public int IdManoObra { get; set; } 
            public string? Descripcion { get; set; } 
        }
    }

        public class ManoObraItemViewModel 
    { 
        public int IdManoObra { get; set; }
        public string? Descripcion { get; set; } 
    }

    /// <summary>
    /// ViewModel para la vista de Detalle General del Plan Preventivo (Nivel 1).
    /// </summary>
    public class DetalleGeneralViewModel
    {
        public int IdPreventivo { get; set; }
        public string? NombrePreventivo { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public DateTime FechaPrev { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal IvaTotal { get; set; }
        public decimal CostoTotal { get; set; }

        public List<DetPrevItemViewModel> Detalles { get; set; } = new();
    }

    public class DetPrevItemViewModel
    {
        public int IdPrevDet { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Iva { get; set; }
        public decimal CostoTotal { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }

    /// <summary>
    /// ViewModel para la vista del Detalle de Mano de Obra por Vehículo (Nivel 2).
    /// </summary>
    public class DetPrevMoItemViewModel
    {
        public int IdPrevMo { get; set; }
        public string? NombrePreventivo { get; set; }
        public string? ManoObra { get; set; }
        public decimal? Iva { get; set; }
        public decimal CostoTotalUnitario { get; set; }
        public DateTime FechaPrev { get; set; }
    }
}