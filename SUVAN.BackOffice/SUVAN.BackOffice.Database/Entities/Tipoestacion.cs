using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Tipoestacion
{
    public int Idtipoestacion { get; set; }

    public string? Nombre { get; set; }

    public ulong? Activo { get; set; }

    public virtual ICollection<RutaParadum> RutaParada { get; set; } = new List<RutaParadum>();
}
