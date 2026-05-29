using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class GrupoReparacion
{
    public int IdGrupo { get; set; }

    public string Descripcion { get; set; } = null!;

    public string? Rubro { get; set; }

    public string? Subrubro { get; set; }

    public virtual ICollection<TipoReparacion> TipoReparacions { get; set; } = new List<TipoReparacion>();
}
