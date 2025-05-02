using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TarifaEscalonadum
{
    public int ParadaIdparada { get; set; }

    public int ParadaIdparada1 { get; set; }

    public int EmpresaIdempresa { get; set; }

    public int RutaIdruta { get; set; }

    public decimal? Precio { get; set; }

    public virtual Paradum ParadaIdparada1Navigation { get; set; } = null!;

    public virtual Paradum ParadaIdparadaNavigation { get; set; } = null!;
}
