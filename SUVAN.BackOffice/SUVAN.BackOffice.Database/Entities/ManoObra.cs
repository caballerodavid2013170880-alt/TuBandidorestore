using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class ManoObra
{
    public int IdManoObra { get; set; }

    public string? DescripcionManoobra { get; set; }

    public virtual ICollection<DetPrev> DetPrevs { get; set; } = new List<DetPrev>();
}
