using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaInspeccion
{
    public ulong IdLlantaInspeccion { get; set; }

    public ulong IdLlanta { get; set; }

    public ulong IdTipoInspeccion { get; set; }

    public ulong? IdLlantaAsignacion { get; set; }

    public DateTime FechaInspeccion { get; set; }

    public uint? KilometrajeLlanta { get; set; }

    public decimal? ProfundidadMm { get; set; }

    public decimal? PresionPsi { get; set; }

    public ushort IdEstadoInspeccion { get; set; }

    public ushort IdConclusionInspeccion { get; set; }

    public bool RequiereAccion { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; }

    public uint CreadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public uint? ModificadoPor { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public uint? EliminadoPor { get; set; }

    public virtual LlantaConclusionInspeccion IdConclusionInspeccionNavigation { get; set; } = null!;

    public virtual LlantaEstadoInspeccion IdEstadoInspeccionNavigation { get; set; } = null!;

    public virtual LlantaAsignacion? IdLlantaAsignacionNavigation { get; set; }

    public virtual Llantum IdLlantaNavigation { get; set; } = null!;

    public virtual LlantaTipoInspeccion IdTipoInspeccionNavigation { get; set; } = null!;
}
