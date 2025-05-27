using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CausaMantenimiento
{
    public int IdCausamantenimiento { get; set; }

    public string Nombre { get; set; } = null!;

    public int PreventivoIdpreventivo { get; set; }

    public virtual Preventivo PreventivoIdpreventivoNavigation { get; set; } = null!;
}
