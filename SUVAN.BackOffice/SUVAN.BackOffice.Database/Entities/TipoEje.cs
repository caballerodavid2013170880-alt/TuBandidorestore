using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TipoEje
{
    public int IdTipoEje { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();
}
