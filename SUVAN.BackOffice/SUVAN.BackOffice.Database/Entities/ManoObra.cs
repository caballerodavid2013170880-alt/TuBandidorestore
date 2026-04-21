using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class ManoObra
{
    public int IdManoObra { get; set; }

    public string Descripcion { get; set; } = null!;
}
