using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class MantenimientoDetalle
{
    public int Idmantenimientodetalle { get; set; }

    public string Reparacion { get; set; } = null!;

    public float Cantidad { get; set; }

    public string Referencia { get; set; } = null!;

    public float Valor { get; set; }

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
}
