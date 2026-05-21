using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class Variableglobal
{
    public int VariableIdvariable { get; set; }

    public string? Valor { get; set; }

    public virtual Variable VariableIdvariableNavigation { get; set; } = null!;
}
