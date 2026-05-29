using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Tipovariable
{
    public int Idtipovariable { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Variable> Variables { get; set; } = new List<Variable>();
}
