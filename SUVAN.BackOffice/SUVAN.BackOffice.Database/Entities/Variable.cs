using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Variable
{
    public int Idvariable { get; set; }

    public string? Codigo { get; set; }

    public string? Descripcion { get; set; }

    public int? TipovariableIdtipovariable { get; set; }

    public string? Lista { get; set; }

    public virtual Tipovariable? TipovariableIdtipovariableNavigation { get; set; }

    public virtual ICollection<Variableempresa> Variableempresas { get; set; } = new List<Variableempresa>();

    public virtual Variableglobal? Variableglobal { get; set; }
}
