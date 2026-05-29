using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Tipofavorito
{
    public int Idtipofavorito { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
}
