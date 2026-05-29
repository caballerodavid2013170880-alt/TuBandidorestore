using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Depositosdisponible
{
    public int IdDeposito { get; set; }

    public int ZonaId { get; set; }

    public string DepositoNombre { get; set; } = null!;

    public bool Activo { get; set; }

    public string Dirección { get; set; } = null!;

    public string Ciudad { get; set; } = null!;

    public string Responsable { get; set; } = null!;

    public string Teléfono { get; set; } = null!;

    public string? LocFor { get; set; }

    public string? RPerson { get; set; }

    public int IdEmpresa { get; set; }

    public string NombreCorto { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public string Cp { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<Mecanico> Mecanicos { get; set; } = new List<Mecanico>();

    public virtual ICollection<Siniestro> Siniestros { get; set; } = new List<Siniestro>();

    public virtual ICollection<Taller> Tallers { get; set; } = new List<Taller>();

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();

    public virtual Zona Zona { get; set; } = null!;
}
