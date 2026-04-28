using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Plantum
{
    public short IdRegion { get; set; }

    public short IdPlanta { get; set; }

    public string? Nombre { get; set; }

    public string? Libreria { get; set; }

    public short? IdEmp { get; set; }

    // Relaciones que simulan las FK
    public virtual Region IdRegionNavigation { get; set; } = null!;
}
