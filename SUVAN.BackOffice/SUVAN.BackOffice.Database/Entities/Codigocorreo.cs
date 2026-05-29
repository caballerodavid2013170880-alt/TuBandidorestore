using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Codigocorreo
{
    public int Idcodigocorreo { get; set; }

    public int? CodigodescuentoIdcodigodescuento { get; set; }

    public string? Email { get; set; }

    public virtual Codigodescuento? CodigodescuentoIdcodigodescuentoNavigation { get; set; }
}
