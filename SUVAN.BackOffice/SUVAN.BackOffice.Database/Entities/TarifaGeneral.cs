using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TarifaGeneral
{
    public int? EmpresaIdempresa { get; set; }

    public int RutaIdruta { get; set; }

    public decimal? Precio { get; set; }

    public virtual Rutum RutaIdrutaNavigation { get; set; } = null!;
}
