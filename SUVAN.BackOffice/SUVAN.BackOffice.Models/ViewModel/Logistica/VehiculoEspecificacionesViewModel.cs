using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class VehiculoEspecificacionesViewModel
    {
        public short IdEspecificaciones { get; set; }

        public int? IdMarca { get; set; }

        public int? IdModelo { get; set; }

        public short? TipoCombustible { get; set; }

        public short? Pallets { get; set; }

        public double? RenEsp { get; set; }

        public double? CapacidadCombu { get; set; }

        public double? CapacidadAceite { get; set; }

        public string? TipoMotor { get; set; }

        public string? PotenciaMotor { get; set; }

        public string? PulCub { get; set; }

        public short? NoCilindros { get; set; }

        public double? Ancho { get; set; }

        public double? Largo { get; set; }

        public double? Altura { get; set; }

        public float? PesoBruto { get; set; }

        public float? CargaPorEje { get; set; }

        public float? CargaMax { get; set; }

        public short? LlantasRepuesto { get; set; }

        public short? TotalLlantas { get; set; }

        public short? DimensionLlantas { get; set; }

        public string? Transmision { get; set; }

        public string? Traccion { get; set; }

        public string? Origen { get; set; }

        public short? TipoEje { get; set; }

        public double? MetrosCubCarga { get; set; }

        public double? ToneladasCarga { get; set; }

        public string? Observaciones { get; set; }

        public string MarcaJson { get; set; } = string.Empty;

        public List<MarcaEspecifiViewModel> Marcas { get; set; } = new();

        public List<ModeloEspecifiViewModel> Modelos { get; set; } = new();

        public List<VehiculoEspecifiDescripcionViewModel> Descripcion { get; set; } = new();

        public class ModeloEspecifiViewModel
        {
            public short IdMarca { get; set; }

            public int? IdModelo { get; set; }

            public string Descripcion { get; set; } = null!;

            public string DescripcionModeloId => $"{IdModelo} - {Descripcion}";

            public List<MarcaEspecifiViewModel> MarcasView { get; set; } = new();
        }

        public class MarcaEspecifiViewModel
        {
            public int? IdMarca { get; set; }

            public string Descripcion { get; set; } = null!;

            public string DescripcionMarcaId => $"{IdMarca} - {Descripcion}";

            public List<ModeloEspecifiViewModel> Modelos { get; set; } = new();
        }

        public class VehiculoEspecifiDescripcionViewModel
        {
            public int? IdMarca { get; set; }

            public string DescripcionMarca { get; set; } = null!;

            public int? IdModelo { get; set; }

            public string DescripcionModelo { get; set; } = null!;

            public double? CapacidadCombu { get; set; }

            public string? TipoMotor { get; set; }

            public short? IdEspecificaciones { get; set; }
        }
    }
}
