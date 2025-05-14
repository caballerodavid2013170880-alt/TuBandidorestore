using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Depositosdisponible
{
    public int IdDeposito { get; set; }

    public int ZonaId { get; set; }

    public string ZonaNombre { get; set; } = null!;

    public string DepositoNombre { get; set; } = null!;

    public int TalleId { get; set; }

    public string TallerNombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<Mecanico> Mecanicos { get; set; } = new List<Mecanico>();

    public virtual ICollection<Siniestro> Siniestros { get; set; } = new List<Siniestro>();
}
