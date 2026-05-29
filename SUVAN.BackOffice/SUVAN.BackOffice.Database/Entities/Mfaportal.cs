using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Mfaportal
{
    public int AdminIdadmin { get; set; }

    public string? Codigo { get; set; }

    public DateTime? Expira { get; set; }

    public virtual Admin AdminIdadminNavigation { get; set; } = null!;
}
