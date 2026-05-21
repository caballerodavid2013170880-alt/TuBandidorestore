using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class TipoCom
{
    public int IdComb { get; set; }

    public string Nombre { get; set; } = null!;

    public float PrecLit { get; set; }

    public string Rubro { get; set; } = null!;

    public string? Subrubro { get; set; }

    public short CUnidad { get; set; }

    public virtual ICollection<CombCarga> CombCargas { get; set; } = new List<CombCarga>();

    public virtual ICollection<RendMe> RendMes { get; set; } = new List<RendMe>();

    public virtual ICollection<TarCom> TarComs { get; set; } = new List<TarCom>();
}
