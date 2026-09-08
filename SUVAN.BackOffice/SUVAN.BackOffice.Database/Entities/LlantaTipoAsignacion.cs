using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaTipoAsignacion
{
    public ushort IdTipoAsignacion { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public virtual ICollection<LlantaAsignacion> LlantaAsignacions { get; set; } = new List<LlantaAsignacion>();
}
