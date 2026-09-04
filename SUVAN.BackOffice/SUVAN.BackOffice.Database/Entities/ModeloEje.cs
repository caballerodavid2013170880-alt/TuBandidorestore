using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class ModeloEje
{
    public int IdModeloEje { get; set; }

    public int IdModelo { get; set; }

    public int IdTipoEje { get; set; }

    public ushort NumeroEje { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int? CreadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? ModificadoPor { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public int? EliminadoPor { get; set; }

    public virtual Modelo IdModeloNavigation { get; set; } = null!;

    public virtual TipoEje IdTipoEjeNavigation { get; set; } = null!;
}
