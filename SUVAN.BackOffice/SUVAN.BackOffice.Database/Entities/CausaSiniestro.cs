using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CausaSiniestro
{
    public short IdCausaSiniestro { get; set; }

    public string? Descripcion { get; set; }
}
