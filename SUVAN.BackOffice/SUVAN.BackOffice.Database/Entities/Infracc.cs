using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Infracc
{
    public short CRegion { get; set; }

    public short CPlanta { get; set; }

    public short CZona { get; set; }

    public short CDeposi { get; set; }

    public short CInfracc { get; set; }

    public DateTime? Fecha { get; set; }

    public string? COper { get; set; }

    public string? CVehic { get; set; }

    public short? CCausa { get; set; }

    public double? MonInf { get; set; }

    public double? PagOp { get; set; }

    public double? PagEmp { get; set; }

    public string? Ubica { get; set; }

    public string? Comenta { get; set; }

    public string? Localida { get; set; }

    public DateTime? FLimPa { get; set; }

    public DateTime? FPago { get; set; }

    public string? Status { get; set; }

    public short? CNegoc { get; set; }

    public string? CRuta { get; set; }
}
