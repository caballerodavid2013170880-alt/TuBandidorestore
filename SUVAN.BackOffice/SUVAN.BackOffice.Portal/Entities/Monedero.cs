using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class Monedero
{
    public int UsuarioIdusuario { get; set; }

    public decimal? Saldo { get; set; }

    public virtual ICollection<Recarga> Recargas { get; set; } = new List<Recarga>();

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;
}
