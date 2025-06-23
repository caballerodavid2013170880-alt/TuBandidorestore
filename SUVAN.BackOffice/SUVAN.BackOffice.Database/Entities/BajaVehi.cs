using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class BajaVehi
{
    public int IdBaja { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<CausaBaja> CausaBajas { get; set; } = new List<CausaBaja>();

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();
}
