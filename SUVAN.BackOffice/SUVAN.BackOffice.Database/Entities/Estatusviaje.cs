using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Estatusviaje
{
    public sbyte Idestatusviaje { get; set; }

    public string? Estatusviajecol { get; set; }

    public virtual ICollection<CorridaAsignacion> CorridaAsignacions { get; set; } = new List<CorridaAsignacion>();

    public virtual ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
}
