using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LiquidacionCabecera
{
    public int IdLiquidacion { get; set; }

    public int Idconductor { get; set; }

    public DateTime FechaEmision { get; set; }

    public DateTime FechaInico { get; set; }

    public DateTime FechaFin { get; set; }

    public decimal? MontoPagado { get; set; }

    public decimal? MontoComision { get; set; }

    public virtual Conductor IdconductorNavigation { get; set; } = null!;

    public virtual ICollection<LiquidacionDetalle> LiquidacionDetalles { get; set; } = new List<LiquidacionDetalle>();
}
