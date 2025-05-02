using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Puntovirtual
{
    public int Idpuntovirtual { get; set; }

    public int RutaParadaRutaIdruta { get; set; }

    public int RutaParadaParadaIdparada { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public int? Orden { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual RutaParadum RutaParada { get; set; } = null!;
}
