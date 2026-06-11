using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class Tipopromocion
{
    public int Idtipopromocion { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Promocion> Promocions { get; set; } = new List<Promocion>();
}
