using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Codigodescuento
{
    public int Idcodigodescuento { get; set; }

    public string? Codigo { get; set; }

    public DateTime? Vigenciadesde { get; set; }

    public DateTime? Vigenciahasta { get; set; }

    public decimal? Cantidad { get; set; }

    public int TipodescuentoIdtipodescuento1 { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public int TipocodigodescuentoIdtipocodigodescuento { get; set; }

    public virtual ICollection<Codigocorreo> Codigocorreos { get; set; } = new List<Codigocorreo>();

    public virtual Tipocodigodescuento TipocodigodescuentoIdtipocodigodescuentoNavigation { get; set; } = null!;

    public virtual Tipodescuento TipodescuentoIdtipodescuento1Navigation { get; set; } = null!;
}
