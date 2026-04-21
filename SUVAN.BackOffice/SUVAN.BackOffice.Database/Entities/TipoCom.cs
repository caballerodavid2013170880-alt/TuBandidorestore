using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TipoCom
{
    public short IdComb { get; set; }

    public string? Descrip { get; set; }

    public double? PrecLit { get; set; }

    public string? Rubro { get; set; }

    public string? Subrubro { get; set; }
}
