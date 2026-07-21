using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaEstadoRenovado
{
    public ushort IdEstadoRenovado { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public virtual ICollection<LlantaRenovado> LlantaRenovados { get; set; } = new List<LlantaRenovado>();
}
