using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Tipodescuento
{
    public int Idtipodescuento { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Codigodescuento> Codigodescuentos { get; set; } = new List<Codigodescuento>();

    public virtual ICollection<Promocion> Promocions { get; set; } = new List<Promocion>();
}
