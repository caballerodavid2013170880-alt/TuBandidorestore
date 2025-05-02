using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CorridaAsignacionParadum
{
    public int CorridaAsignacionIdcorridaAsignacion { get; set; }

    public int ParadaIdparada { get; set; }

    public int? Suben { get; set; }

    public int? Bajan { get; set; }

    public int? Espacios { get; set; }

    public int? EstatusestacionIdestatusestacion { get; set; }

    public int? Subieron { get; set; }

    public int? Bajaron { get; set; }

    public virtual Estatusestacion? EstatusestacionIdestatusestacionNavigation { get; set; }
}
