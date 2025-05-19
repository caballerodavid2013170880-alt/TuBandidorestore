using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Taller
{
    public int IdTaller { get; set; }

    public int ZonaIdzona { get; set; }

    public string NombreTaller { get; set; } = null!;

    public virtual ICollection<Depositosdisponible> Depositosdisponibles { get; set; } = new List<Depositosdisponible>();

    public virtual Zona ZonaIdzonaNavigation { get; set; } = null!;
}
