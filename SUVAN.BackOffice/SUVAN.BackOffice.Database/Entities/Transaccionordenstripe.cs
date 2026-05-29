using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Transaccionordenstripe
{
    public int TransaccionOrdenStripeId { get; set; }

    public string NumeroOrdern { get; set; } = null!;

    public string NumeroPeticion { get; set; } = null!;

    public string StripeId { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }
}
