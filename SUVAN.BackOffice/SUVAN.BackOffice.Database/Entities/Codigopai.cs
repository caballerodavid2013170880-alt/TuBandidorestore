using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Codigopai
{
    public int Idcodigopais { get; set; }

    public string? Pais { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual ICollection<Conductor> Conductors { get; set; } = new List<Conductor>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
