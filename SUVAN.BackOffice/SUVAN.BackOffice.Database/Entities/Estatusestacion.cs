using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Estatusestacion
{
    public int Idestatusestacion { get; set; }

    public string? Nombre { get; set; }

    public ulong? Activo { get; set; }

    public virtual ICollection<CorridaAsignacionParadum> CorridaAsignacionParada { get; set; } = new List<CorridaAsignacionParadum>();
}
