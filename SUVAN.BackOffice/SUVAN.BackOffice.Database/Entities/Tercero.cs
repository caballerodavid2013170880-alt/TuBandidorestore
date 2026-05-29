using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Tercero
{
    public int CSinies { get; set; }

    public short CTercero { get; set; }

    public short? CRegion { get; set; }

    public short? CPlanta { get; set; }

    public short? CZona { get; set; }

    public short? CDeposi { get; set; }

    public short? Tercero1 { get; set; }

    public string? Descrip { get; set; }

    public short? AnioBie { get; set; }

    public string? MarMod { get; set; }

    public string? Serie { get; set; }

    public string? Placa { get; set; }

    public string? Propiet { get; set; }

    public string? Domicil { get; set; }

    public short? EdadPer { get; set; }

    public string? Sexo { get; set; }

    public short? TipoDan { get; set; }

    public short? EstFisi { get; set; }

    public string? Asegura { get; set; }

    public string? Ajustad { get; set; }

    public float? Costo { get; set; }

    public float? CostEmp { get; set; }

    public float? CostSeg { get; set; }

    public float? CostOpe { get; set; }
}
