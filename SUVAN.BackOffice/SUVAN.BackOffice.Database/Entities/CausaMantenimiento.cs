using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CausaMantenimiento
{
    public int IdCausamantenimiento { get; set; }

    public string? Descripcion { get; set; }
}
