using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class VehiculoEje
{
    public int IdVehiculoEje { get; set; }

    public int IdVehiculo { get; set; }

    public int IdTipoEje { get; set; }

    public ushort NumeroEje { get; set; }

    public bool? Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public uint CreadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public uint? ModificadoPor { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public uint? EliminadoPor { get; set; }

    public virtual TipoEje IdTipoEjeNavigation { get; set; } = null!;

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;

    public virtual ICollection<LlantaAsignacion> LlantaAsignacions { get; set; } = new List<LlantaAsignacion>();
}
