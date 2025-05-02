using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class EmpresaRutum
{
    public int EmpresaIdempresa { get; set; }

    public int RutaIdruta { get; set; }

    public int TipotarifaIdtipotarifa { get; set; }

    public DateTime? Recharegistro { get; set; }

    public ulong? Activo { get; set; }
}
