using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class GasIave
{
    public int IdIave { get; set; }

    public short IdRegion { get; set; }

    public short IdPlanta { get; set; }

    public short IdZona { get; set; }

    public short IdDeposito { get; set; }

    public string Tarjeta { get; set; } = null!;

    public string? NumEco { get; set; }

    public DateTime FCruce { get; set; }

    public string HCruce { get; set; } = null!;

    public string Caseta { get; set; } = null!;

    public double? Importe { get; set; }

    public int? Clase { get; set; }

    public string? Consecar { get; set; }

    public DateTime? FechaApli { get; set; }

    public DateTime? FechaRegistro { get; set; }
}
