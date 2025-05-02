using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Tipocontenido
{
    public int Idtipocontenido { get; set; }

    public string? Nombre { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual ICollection<Contenido> Contenidos { get; set; } = new List<Contenido>();
}
