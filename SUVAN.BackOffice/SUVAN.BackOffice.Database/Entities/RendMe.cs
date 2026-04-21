using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class RendMe
{
    public short? Anio { get; set; }

    public short? Mes { get; set; }

    public string IdVehic { get; set; } = null!;

    public double? TotalLitros { get; set; }

    public double? TotalKms { get; set; }

    public float? RendPromedio { get; set; }

    public double? TotalImporte { get; set; }
}
