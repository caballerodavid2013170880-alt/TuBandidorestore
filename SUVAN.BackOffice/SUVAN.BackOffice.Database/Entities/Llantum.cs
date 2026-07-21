using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Llantum
{
    public ulong IdLlanta { get; set; }

    public string CodigoLlanta { get; set; } = null!;

    public string NumeroSerieDot { get; set; } = null!;

    public uint IdModeloLlanta { get; set; }

    public ushort IdEstadoLlanta { get; set; }

    public uint? IdEmpresa { get; set; }

    public uint? IdRegion { get; set; }

    public uint? IdPlanta { get; set; }

    public uint? IdZona { get; set; }

    public uint? IdDeposito { get; set; }

    public decimal? PresionMinimaPsi { get; set; }

    public decimal? PresionMaximaPsi { get; set; }

    public DateOnly? FechaFabricacion { get; set; }

    public decimal? ProfundidadOriginalMm { get; set; }

    public uint? VidaUtilEstimadaKm { get; set; }

    public decimal? CostoAdquisicion { get; set; }

    public DateOnly? FechaAdquisicion { get; set; }

    public string? Observaciones { get; set; }

    public bool Eliminado { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public uint? EliminadoPor { get; set; }

    public string? MotivoEliminacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public uint CreadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public uint? ModificadoPor { get; set; }

    public virtual LlantaEstado IdEstadoLlantaNavigation { get; set; } = null!;

    public virtual ICollection<LlantaAsignacion> LlantaAsignacions { get; set; } = new List<LlantaAsignacion>();

    public virtual ICollection<LlantaInspeccion> LlantaInspeccions { get; set; } = new List<LlantaInspeccion>();

    public virtual ICollection<LlantaRenovado> LlantaRenovados { get; set; } = new List<LlantaRenovado>();
}
