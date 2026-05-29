using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Mantenimiento
{
    public int IdMantenimiento { get; set; }

    public int IdDeposito { get; set; }

    public int IdTiposervicio { get; set; }

    public int OrdenServicio { get; set; }

    public int? IdPreventivo { get; set; }

    public int IdVehiculo { get; set; }

    public int? IdTaller { get; set; }

    public int? IdMecanico { get; set; }

    public DateTime? FechaProgramacion { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public string? HoraIngreso { get; set; }

    public DateTime? FinTaller { get; set; }

    public string? HoraFinTaller { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public string? HoraEntrega { get; set; }

    public string? NivelTanque { get; set; }

    public int? Kilometraje { get; set; }

    public string? Genero { get; set; }

    public string? Estatus { get; set; }

    public short? Color { get; set; }

    public string? Comentarios { get; set; }

    public float? TotalOrden { get; set; }

    public short? Iva { get; set; }

    public string? Recibio { get; set; }

    public string? Entrego { get; set; }

    public DateTime? FechaCaptura { get; set; }

    public short? TipoAfe { get; set; }

    public string? Vale { get; set; }

    public int? IdTipoMantenimiento { get; set; }

    public int? FoReOp { get; set; }

    public short? Refac { get; set; }

    public string? ObservacionesOperador { get; set; }

    public string? ComentariosTecnico { get; set; }

    public string? Refacciones { get; set; }

    public virtual Depositosdisponible IdDepositoNavigation { get; set; } = null!;

    public virtual Mecanico? IdMecanicoNavigation { get; set; }

    public virtual Taller? IdTallerNavigation { get; set; }

    public virtual TipoMantenimiento? IdTipoMantenimientoNavigation { get; set; }

    public virtual TipoServicio IdTiposervicioNavigation { get; set; } = null!;

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
}
