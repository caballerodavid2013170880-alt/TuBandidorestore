using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TipoAutorizacion
{
    public short CDeposito { get; set; }

    public short IdTipo { get; set; }

    public string? Descripcion { get; set; }

    public ulong? SeAutoriza { get; set; }
}
