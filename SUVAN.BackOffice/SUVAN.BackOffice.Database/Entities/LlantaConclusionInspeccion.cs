using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaConclusionInspeccion
{
    public ushort IdConclusionInspeccion { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public virtual ICollection<LlantaInspeccion> LlantaInspeccions { get; set; } = new List<LlantaInspeccion>();
}
