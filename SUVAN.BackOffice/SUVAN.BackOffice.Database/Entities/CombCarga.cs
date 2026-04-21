using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CombCarga
{
    public int IdCarga { get; set; }

    public string IdVehic { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public TimeOnly? Hora { get; set; }

    public string? FolioNota { get; set; }

    public string? Factura { get; set; }

    public double? Importe { get; set; }

    public double? Litros { get; set; }

    public double? KmAnterior { get; set; }

    public double? KmActual { get; set; }

    public double? KmRecorridos { get; set; }

    public float? Rendimiento { get; set; }

    public double? CostoXLt { get; set; }

    public short? IdComb { get; set; }

    public sbyte? Traspasar { get; set; }

    public string? Espec { get; set; }

    public DateTime? FRegistro { get; set; }

    public short? IdRegion { get; set; }

    public short? IdPlanta { get; set; }

    public short? IdZona { get; set; }

    public short? IdDeposito { get; set; }
}
