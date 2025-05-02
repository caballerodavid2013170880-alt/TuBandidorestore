using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Datosfacturacioncomprobante
{
    public int Iddatosfacturacioncomprobante { get; set; }

    public string? Serie { get; set; }

    public int? Folioinicial { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public int EmpresaIdempresa { get; set; }

    public virtual Empresa EmpresaIdempresaNavigation { get; set; } = null!;
}
