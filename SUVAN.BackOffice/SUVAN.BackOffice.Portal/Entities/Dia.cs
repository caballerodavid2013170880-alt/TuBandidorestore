using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class Dia
{
    public string Iddias { get; set; } = null!;

    public string? Nombre { get; set; }

    public int? Orden { get; set; }

    public virtual ICollection<CorridaDia> CorridaDia { get; set; } = new List<CorridaDia>();
}
