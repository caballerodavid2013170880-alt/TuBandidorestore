using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Region
{
    public short IdEmpresa { get; set; }

    public short IdRegion { get; set; }

    public string? Nombre { get; set; }

    // Relación con PLanta
    //public virtual ICollection<Plantum> Plantum { get; set; } = new List<Plantum>();
}
