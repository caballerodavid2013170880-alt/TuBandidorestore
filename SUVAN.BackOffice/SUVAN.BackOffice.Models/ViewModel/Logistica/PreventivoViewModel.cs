using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class PreventivoViewModel
    {
        public int Idpreventivo { get; set; }

        public int Idempresa { get; set; }

        [Required(ErrorMessage = "El Nombre del Preventivo es requerido")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 70 caracteres")]
        public string? NombrePreventivo { get; set; }

        public string? ObservacionesPreventivo { get; set; }

        [Required(ErrorMessage = "La Planta es requerida")]
        public int IdPlanta { get; set; }

        [Required(ErrorMessage = "El Depósito es requerido")]
        public int IdDeposito { get; set; }

        [Required(ErrorMessage = "Los meses son requeridos")]
        [StringLength(30, ErrorMessage = "La longitud máxima es de 30 caracteres")]
        public string? Meses { get; set; }

        [Required(ErrorMessage = "La Marca es requerida")]
        public short? IdMarca { get; set; }

        [Required(ErrorMessage = "El Modelo es requerido")]
        public int IdModelo { get; set; }

        // Variable temporal para capturar la Mano de Obra
        [Required(ErrorMessage = "La Mano de Obra es requerida")]
        public int IdManoObra { get; set; }

        public decimal? CostoTotal { get; set; }

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
        {
            public short IdMarca { get; set; }
            public string? Nombre { get; set; }
        }

        public class ModeloItemViewModel
        {
            public int IdModelo { get; set; }
            public string? Nombre { get; set; }
        }

        public class ManoObraItemViewModel
        {
            public int IdManoObra { get; set; }
            public string? Descripcion { get; set; }
        }
    }
}