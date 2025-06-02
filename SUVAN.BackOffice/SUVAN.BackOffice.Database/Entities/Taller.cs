using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Taller
{
    public int IdTaller { get; set; }

    public int ZonaIdzona { get; set; }

    public string NombreTaller { get; set; } = null!;

    public int IdDeposito { get; set; }

    public string? TTaller { get; set; }

    public ulong? Central { get; set; }

    public string Domicilio { get; set; } = null!;

    public string Contacto { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? Iva { get; set; }

    public ulong? Refaccion { get; set; }

    public float? ValorUnitario { get; set; }

    public virtual Depositosdisponible IdDepositoNavigation { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual Zona ZonaIdzonaNavigation { get; set; } = null!;
}
