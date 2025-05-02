using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Logtransaccion
{
    public int Idlogtransaccion { get; set; }

    public string? TransaccionNumeroordenpay { get; set; }

    public string? Respuesta { get; set; }

    public DateTime? Fecharegistro { get; set; }
}
