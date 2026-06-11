using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class Logcancelacionviaje
{
    public int Idlogcancelacionviajes { get; set; }

    public int ViajeIdviaje { get; set; }

    public decimal? Saldoabonadaamonedero { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual Viaje ViajeIdviajeNavigation { get; set; } = null!;
}
