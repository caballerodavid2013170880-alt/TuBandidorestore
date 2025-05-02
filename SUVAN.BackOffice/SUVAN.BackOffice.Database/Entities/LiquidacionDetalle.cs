using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LiquidacionDetalle
{
    public int IdLiquidacion { get; set; }

    public int IdCorridaAsignacion { get; set; }

    public bool? Logical { get; set; }

    public virtual CorridaAsignacion IdCorridaAsignacionNavigation { get; set; } = null!;

    public virtual LiquidacionCabecera IdLiquidacionNavigation { get; set; } = null!;
}
