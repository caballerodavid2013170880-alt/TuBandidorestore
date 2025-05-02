using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Datosfacturacionemisor
{
    public int Iddatosfacturacionemisor { get; set; }

    public int EmpresaIdempresa { get; set; }

    public string? Regimenfiscal { get; set; }

    public string? Nombre { get; set; }

    public string? Serie { get; set; }

    public int? Folio { get; set; }

    public string? LugarexpedicionCp { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual Empresa EmpresaIdempresaNavigation { get; set; } = null!;
}
