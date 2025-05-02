using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Variableempresa
{
    public int VariableIdvariable { get; set; }

    public int EmpresaIdempresa { get; set; }

    public string? Valor { get; set; }

    public virtual Empresa EmpresaIdempresaNavigation { get; set; } = null!;

    public virtual Variable VariableIdvariableNavigation { get; set; } = null!;
}
