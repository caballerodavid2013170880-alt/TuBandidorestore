using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class PromocionRutum
{
    public int PromocionIdpromocion { get; set; }

    public int RutaIdruta { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual Promocion PromocionIdpromocionNavigation { get; set; } = null!;

    public virtual Rutum RutaIdrutaNavigation { get; set; } = null!;
}
