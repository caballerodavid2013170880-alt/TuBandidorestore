using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CorridaDia
{
    public string DiasIddias { get; set; } = null!;

    public int CorridaIdcorrida { get; set; }

    public ulong? Activo { get; set; }

    public virtual Corridum CorridaIdcorridaNavigation { get; set; } = null!;

    public virtual Dia DiasIddiasNavigation { get; set; } = null!;
}
