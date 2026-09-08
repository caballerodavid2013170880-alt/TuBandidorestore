using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaAsignacionBitacora
{
    public ulong IdLlantaAsignacionBitacora { get; set; }

    public Guid? IdOperacion { get; set; }

    public ulong IdLlanta { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public ulong? IdLlantaAsignacionOrigen { get; set; }

    public ulong? IdVehiculoOrigen { get; set; }

    public ulong? IdVehiculoEjeOrigen { get; set; }

    public ushort? PosicionOrigen { get; set; }

    public ulong? IdLlantaAsignacionDestino { get; set; }

    public ulong? IdVehiculoDestino { get; set; }

    public ulong? IdVehiculoEjeDestino { get; set; }

    public ushort? PosicionDestino { get; set; }

    public uint? Kilometraje { get; set; }

    public string? Observaciones { get; set; }

    public uint CreadoPor { get; set; }

    public DateTime FechaMovimiento { get; set; }
}
