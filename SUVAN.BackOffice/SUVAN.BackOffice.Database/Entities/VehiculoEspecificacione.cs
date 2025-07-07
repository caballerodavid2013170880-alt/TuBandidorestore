using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class VehiculoEspecificacione
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
}
