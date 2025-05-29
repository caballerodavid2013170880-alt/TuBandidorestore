using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Zona
{
    public int IdZona { get; set; }

    public string NombreZona { get; set; } = null!;

    public string? Rfc { get; set; }

    public string? Domicilio { get; set; }

    public string? Telefono1 { get; set; }

    public string? Telefono2 { get; set; }

    public string? Responsable { get; set; }

    public DateTime? FechaApertura { get; set; }

    public int? IdEmpresa { get; set; }

    public ulong? Activo { get; set; }

    public virtual ICollection<Depositosdisponible> Depositosdisponibles { get; set; } = new List<Depositosdisponible>();

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual ICollection<Taller> Tallers { get; set; } = new List<Taller>();
}
