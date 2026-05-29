using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class PromocionEmpresa
{
    public int PromocionIdpromocion { get; set; }

    public int EmpresaIdempresa { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual Empresa EmpresaIdempresaNavigation { get; set; } = null!;

    public virtual Promocion PromocionIdpromocionNavigation { get; set; } = null!;
}
