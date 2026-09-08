using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TipoEje
{
    public int IdTipoEje { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public byte NumeroPosiciones { get; set; }

    public bool? EsActivo { get; set; }

    public virtual ICollection<ModeloEje> ModeloEjes { get; set; } = new List<ModeloEje>();

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();

    public virtual ICollection<VehiculoEje> VehiculoEjes { get; set; } = new List<VehiculoEje>();
}
