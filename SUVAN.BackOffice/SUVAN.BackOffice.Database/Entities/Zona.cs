using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Zona
{
    public int IdZona { get; set; }

    public string NombreZona { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public string Domicilio { get; set; } = null!;

    public string Telefono1 { get; set; } = null!;

    public string Telefono2 { get; set; } = null!;

    public string Responsable { get; set; } = null!;

    public DateTime FechaApertura { get; set; }

    public int IdEmpresa { get; set; }

    public ulong Activo { get; set; }

    public virtual ICollection<Depositosdisponible> Depositosdisponibles { get; set; } = new List<Depositosdisponible>();

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual ICollection<Taller> Tallers { get; set; } = new List<Taller>();
}
