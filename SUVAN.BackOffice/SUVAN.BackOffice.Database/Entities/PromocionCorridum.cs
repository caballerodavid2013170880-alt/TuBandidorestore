using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class PromocionCorridum
{
    public int PromocionCorridaid { get; set; }

    public int PromocionIdpromocion { get; set; }

    public int CorridaIdcorrida { get; set; }

    public virtual Corridum CorridaIdcorridaNavigation { get; set; } = null!;

    public virtual Promocion PromocionIdpromocionNavigation { get; set; } = null!;
}
