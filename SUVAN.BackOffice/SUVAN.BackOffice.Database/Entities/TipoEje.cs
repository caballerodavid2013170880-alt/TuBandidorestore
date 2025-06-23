using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TipoEje
{
    public short IdTipoEje { get; set; }

    public string Descripcion { get; set; } = null!;
}
