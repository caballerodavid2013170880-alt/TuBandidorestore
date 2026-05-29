using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Tipotarifa
{
    public int Idtipotarifa { get; set; }

    public string? Nombre { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual ICollection<Rutum> Ruta { get; set; } = new List<Rutum>();
}
