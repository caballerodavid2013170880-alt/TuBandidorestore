using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Personalidadjuridica
{
    public int Idpersonalidadjuridica { get; set; }

    public string? Nombre { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }
}
