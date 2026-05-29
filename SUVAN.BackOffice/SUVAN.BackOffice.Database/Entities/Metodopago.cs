using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Metodopago
{
    public int Idmetodopago { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Recarga> Recargas { get; set; } = new List<Recarga>();

    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();
}
