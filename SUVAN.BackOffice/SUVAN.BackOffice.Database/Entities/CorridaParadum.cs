using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CorridaParadum
{
    public int CorridaIdcorrida { get; set; }

    public int ParadaIdparada { get; set; }

    public TimeOnly? Hora { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual Corridum CorridaIdcorridaNavigation { get; set; } = null!;

    public virtual Paradum ParadaIdparadaNavigation { get; set; } = null!;
}
