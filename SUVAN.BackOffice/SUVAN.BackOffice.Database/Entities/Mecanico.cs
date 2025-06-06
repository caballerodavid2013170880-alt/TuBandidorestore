using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Mecanico
{
    public int IdMecanico { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public int IdDeposito { get; set; }

    public int IdTaller { get; set; }

    public string Puesto { get; set; } = null!;

    public virtual Depositosdisponible IdDepositoNavigation { get; set; } = null!;

    public virtual Taller IdTallerNavigation { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
}
