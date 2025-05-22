using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Mecanico
{
    public int IdMecanico { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Numero { get; set; }

    public DateTime FechaIngreso { get; set; }

    public bool Activo { get; set; }

    public int IdDeposito { get; set; }

    public virtual Depositosdisponible IdDepositoNavigation { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
}
