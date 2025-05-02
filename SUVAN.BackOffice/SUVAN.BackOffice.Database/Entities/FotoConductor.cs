using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class FotoConductor
{
    public int ConductorIdconductor { get; set; }

    public string? Imagen { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual Conductor ConductorIdconductorNavigation { get; set; } = null!;
}
