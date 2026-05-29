using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class FallaAuxilioVial
{
    public uint Idfalla { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Siniestro> Siniestros { get; set; } = new List<Siniestro>();
}
