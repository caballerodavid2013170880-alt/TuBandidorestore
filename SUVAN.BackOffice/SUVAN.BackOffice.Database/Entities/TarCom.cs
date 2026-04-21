using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TarCom
{
    public string IdTarjet { get; set; } = null!;

    public short? IdRegion { get; set; }

    public short? IdPlanta { get; set; }

    public short? IdZona { get; set; }

    public short? IdDeposito { get; set; }

    public string? IdVehic { get; set; }

    public short? IdComb { get; set; }
}
