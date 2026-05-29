using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Usoscfdireceptor
{
    public int Idusoscfdireceptor { get; set; }

    public string? Clave { get; set; }

    public string? Descripcion { get; set; }

    public ulong? Activo { get; set; }

    public ulong? Fisica { get; set; }

    public ulong? Moral { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual ICollection<Datosfacturacionreceptor> Datosfacturacionreceptors { get; set; } = new List<Datosfacturacionreceptor>();

    public virtual ICollection<UsoscfdireceptorRegimenfiscalreceptor> UsoscfdireceptorRegimenfiscalreceptors { get; set; } = new List<UsoscfdireceptorRegimenfiscalreceptor>();
}
