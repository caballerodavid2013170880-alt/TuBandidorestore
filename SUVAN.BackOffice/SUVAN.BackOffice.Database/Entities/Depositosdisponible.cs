using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Depositosdisponible
{
    public int IdDeposito { get; set; }

    public int ZonaId { get; set; }

    public string DepositoNombre { get; set; } = null!;

    public int TallerId { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<Mecanico> Mecanicos { get; set; } = new List<Mecanico>();

    public virtual ICollection<Siniestro> Siniestros { get; set; } = new List<Siniestro>();

    public virtual Taller Taller { get; set; } = null!;

    public virtual Zona Zona { get; set; } = null!;
}
