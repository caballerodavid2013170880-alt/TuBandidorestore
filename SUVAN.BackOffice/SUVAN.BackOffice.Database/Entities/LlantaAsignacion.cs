using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaAsignacion
{
    public ulong IdLlantaAsignacion { get; set; }

    public ulong IdLlanta { get; set; }

    public int IdVehiculo { get; set; }

    public int IdVehiculoEje { get; set; }

    public ushort NumeroPosicion { get; set; }

    public ushort IdTipoAsignacion { get; set; }

    public DateTime FechaAsignacion { get; set; }

    public uint KmVehiculoAsignacion { get; set; }

    public DateTime? FechaRetiro { get; set; }

    public uint? KmVehiculoRetiro { get; set; }

    public ushort? IdMotivoRetiro { get; set; }

    public string? ObservacionesAsignacion { get; set; }

    public string? ObservacionesRetiro { get; set; }

    public bool? Activa { get; set; }

    public DateTime FechaCreacion { get; set; }

    public uint CreadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public uint? ModificadoPor { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public uint? EliminadoPor { get; set; }

    public virtual Llantum IdLlantaNavigation { get; set; } = null!;

    public virtual LlantaMotivoRetiro? IdMotivoRetiroNavigation { get; set; }

    public virtual LlantaTipoAsignacion IdTipoAsignacionNavigation { get; set; } = null!;

    public virtual VehiculoEje IdVehiculoEjeNavigation { get; set; } = null!;

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;

    public virtual ICollection<LlantaInspeccion> LlantaInspeccions { get; set; } = new List<LlantaInspeccion>();
}
