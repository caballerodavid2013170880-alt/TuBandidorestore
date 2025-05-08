using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class MotivoAuxilioVial
{
    public uint Idmotivo { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Siniestro> Siniestros { get; set; } = new List<Siniestro>();
}
