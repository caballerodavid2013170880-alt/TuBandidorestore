using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TipoReparacion
{
    public short IdTipoReparacion { get; set; }

    public string? Descripcion { get; set; }

    public float? Valor { get; set; }

    public int? IdGrupo { get; set; }

    public virtual GrupoReparacion? IdGrupoNavigation { get; set; }

    public virtual ICollection<MantenimientoDet> MantenimientoDets { get; set; } = new List<MantenimientoDet>();
}
