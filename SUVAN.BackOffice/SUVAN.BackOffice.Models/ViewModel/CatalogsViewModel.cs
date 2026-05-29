using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Models.ViewModel
{
    public class CatalogsViewModel
    {
        public class VehiViewModel
        {

            [JsonPropertyOrder(1)]
            public int Id { get; set; }

            [JsonPropertyOrder(2)]
            public string? Placas { get; set; }

            [JsonPropertyOrder(3)]
            public string? Vin { get; set; }

            [JsonPropertyOrder(4)]
            public string? Numeroeconomico { get; set; }

            [JsonPropertyOrder(5)]
            public string? Numeromotor { get; set; }
        }

        public class MarcasViewModel
        {
            [JsonPropertyOrder(1)]
            public int Id { get; set; }

            [JsonPropertyOrder(2)]
            public string Descripcion { get; set; } = null!;

            [JsonPropertyName("Hidden_Modelos")]
            public List<ModelosViewModel> Modelos { get; set; } = new();

        }

        public class ModelosViewModel
        {
            [JsonPropertyOrder(1)]
            public int Id { get; set; }

            [JsonPropertyOrder(2)]
            public string Descripcion { get; set; } = null!;

        }

        public class TiposEjeViewModel
        {
            [JsonPropertyOrder(1)]
            public int Id { get; set; }

            [JsonPropertyOrder(2)]
            public string Descripcion { get; set; } = null!;
        }

        public class TipoVehiculosViewModel
        {
            [JsonPropertyOrder(1)]
            public int Id { get; set; }

            [JsonPropertyOrder(2)]
            public string? Descripcion { get; set; } = null!;

        }

        public class ZonasViewModel
        {
            [JsonPropertyOrder(1)]
            public int Id { get; set; }

            [JsonPropertyOrder(2)]
            public string Descripcion { get; set; } = null!;

            [JsonPropertyName("Hidden_Depsoito")]
            public List<DepositosViewModel> Depositos { get; set; } = new();

        }

        public class DepositosViewModel
        {
            [JsonPropertyOrder(1)]
            public int Id { get; set; }

            [JsonPropertyOrder(2)]
            public string? Descripcion { get; set; } = null!;

        }
    }
}
