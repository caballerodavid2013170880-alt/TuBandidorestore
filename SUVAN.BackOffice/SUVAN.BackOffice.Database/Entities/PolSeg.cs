using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class PolSeg
{
    public short CRegion { get; set; }

    public short CPlanta { get; set; }

    public short CZona { get; set; }

    public short CDeposi { get; set; }

    public short CAseg { get; set; }

    public string CPoliza { get; set; } = null!;

    public string Contrato { get; set; } = null!;

    public short? CTiPol { get; set; }

    public short? CMoneda { get; set; }

    public float? Costo { get; set; }

    public double? PorRob { get; set; }

    public double? PorDm { get; set; }

    public DateTime? Inicio { get; set; }

    public DateTime? Fin { get; set; }

    public short? Pagos { get; set; }

    public string? FrecPag { get; set; }

    public string? Vigente { get; set; }
}
