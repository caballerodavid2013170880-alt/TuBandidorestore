using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CorridaAsignacion
{
    public int IdcorridaAsignacion { get; set; }

    public int CorridaIdcorrida { get; set; }

    public int ConductorIdconductor { get; set; }

    public int VehiculoIdvehiculo { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal? CurrentLat { get; set; }

    public decimal? CurrentLong { get; set; }

    public int? Calificacion { get; set; }

    public string? Mensaje { get; set; }

    public sbyte? EstatusviajeIdestatusviaje { get; set; }

    public int? IdestacionActual { get; set; }

    public virtual Conductor ConductorIdconductorNavigation { get; set; } = null!;

    public virtual Corridum CorridaIdcorridaNavigation { get; set; } = null!;

    public virtual CorridaLiquidacion? CorridaLiquidacion { get; set; }

    public virtual Estatusviaje? EstatusviajeIdestatusviajeNavigation { get; set; }

    public virtual ICollection<LiquidacionDetalle> LiquidacionDetalles { get; set; } = new List<LiquidacionDetalle>();

    public virtual Vehiculo VehiculoIdvehiculoNavigation { get; set; } = null!;

    public virtual ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
}
