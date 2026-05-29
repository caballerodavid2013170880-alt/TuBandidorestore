using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class UsoscfdireceptorRegimenfiscalreceptor
{
    public int IdusoscfdireceptorRegimenfiscalreceptor { get; set; }

    public int Idusoscfdireceptor { get; set; }

    public int Idregimenfiscalreceptor { get; set; }

    public ulong? Activo { get; set; }

    public ulong? Fisica { get; set; }

    public ulong? Moral { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual Regimenfiscalreceptor IdregimenfiscalreceptorNavigation { get; set; } = null!;

    public virtual Usoscfdireceptor IdusoscfdireceptorNavigation { get; set; } = null!;
}
