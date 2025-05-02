using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CorridaLiquidacion
{
    public int IdCorridaAsignacion { get; set; }

    public decimal MontoPagado { get; set; }

    public decimal? MontoComision { get; set; }

    public bool Liquidado { get; set; }

    public virtual CorridaAsignacion IdCorridaAsignacionNavigation { get; set; } = null!;
}
