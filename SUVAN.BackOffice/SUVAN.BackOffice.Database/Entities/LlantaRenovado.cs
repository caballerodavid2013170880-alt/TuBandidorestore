using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaRenovado
{
    public ulong IdLlantaRenovado { get; set; }

    public ulong IdLlanta { get; set; }

    public ushort EsInterno { get; set; }

    public ushort NumeroRenovado { get; set; }

    public DateTime FechaEnvio { get; set; }

    public DateTime? FechaRecepcion { get; set; }

    public DateTime? FechaLiberacion { get; set; }

    public uint KilometrajeRenovado { get; set; }

    public decimal CostoRenovado { get; set; }

    public decimal? ProfundidadInicialMm { get; set; }

    public decimal? ProfundidadFinalMm { get; set; }

    public ushort IdEstadoRenovado { get; set; }

    public DateOnly? GarantiaHasta { get; set; }

    public string? Referencia { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; }

    public uint CreadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public uint? ModificadoPor { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public uint? EliminadoPor { get; set; }

    public virtual LlantaEstadoRenovado IdEstadoRenovadoNavigation { get; set; } = null!;

    public virtual Llantum IdLlantaNavigation { get; set; } = null!;
}
