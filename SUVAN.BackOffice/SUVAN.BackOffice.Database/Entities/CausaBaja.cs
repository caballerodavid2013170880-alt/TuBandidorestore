using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CausaBaja
{
    public int IdCausaBaja { get; set; }

    public int IdBaja { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual BajaVehi IdBajaNavigation { get; set; } = null!;

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();
}
