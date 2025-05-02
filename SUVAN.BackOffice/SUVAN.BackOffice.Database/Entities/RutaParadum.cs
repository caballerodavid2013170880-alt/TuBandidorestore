using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class RutaParadum
{
    public int RutaIdruta { get; set; }

    public int ParadaIdparada { get; set; }

    public int? Orden { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public int? TipoestacionIdtipoestacion { get; set; }

    public int? Tiemposeg { get; set; }

    public virtual Paradum ParadaIdparadaNavigation { get; set; } = null!;

    public virtual ICollection<Puntovirtual> Puntovirtuals { get; set; } = new List<Puntovirtual>();

    public virtual Rutum RutaIdrutaNavigation { get; set; } = null!;

    public virtual Tipoestacion? TipoestacionIdtipoestacionNavigation { get; set; }
}
