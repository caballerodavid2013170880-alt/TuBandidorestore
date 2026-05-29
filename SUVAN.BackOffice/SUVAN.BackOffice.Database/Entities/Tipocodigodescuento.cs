using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Tipocodigodescuento
{
    public int Idtipocodigodescuento { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Codigodescuento> Codigodescuentos { get; set; } = new List<Codigodescuento>();
}
