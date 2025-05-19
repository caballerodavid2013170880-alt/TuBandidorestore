using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Zona
{
    public int IdZona { get; set; }

    public string NombreZona { get; set; } = null!;

    public virtual ICollection<Depositosdisponible> Depositosdisponibles { get; set; } = new List<Depositosdisponible>();

    public virtual ICollection<Taller> Tallers { get; set; } = new List<Taller>();
}
