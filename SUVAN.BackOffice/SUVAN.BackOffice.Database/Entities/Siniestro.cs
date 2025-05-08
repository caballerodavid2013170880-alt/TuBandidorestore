using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Siniestro
{
    public int Idsiniestro { get; set; }

    public int VehiculoIdvehiculo { get; set; }

    public DateOnly FechaReporte { get; set; }

    public TimeOnly HoraReporte { get; set; }

    public string NumeroAuxilio { get; set; } = null!;

    public uint MotivoAuxilioIdIdmotivo { get; set; }

    public uint FallaAuxilioIdIdfalla { get; set; }

    public string Orden { get; set; } = null!;

    public int DepositoIdIdDeposito { get; set; }

    public string Estatus { get; set; } = null!;

    public int ConductorIdconductor { get; set; }

    public string? TipoPoliza { get; set; }

    public decimal? CoberturaDanos { get; set; }

    public DateOnly? VencimientoPoliza { get; set; }

    public bool ResponsabilidadCompartida { get; set; }

    public string LugarSiniestro { get; set; } = null!;

    public string ReporteConclusiones { get; set; } = null!;

    public bool CulpaOperador { get; set; }

    public DateTime Fecharegistro { get; set; }

    public bool Facturado { get; set; }

    public string ReporteOperador { get; set; } = null!;

    public string ReporteAtencion { get; set; } = null!;

    public virtual Conductor ConductorIdconductorNavigation { get; set; } = null!;

    public virtual Depositosdisponible DepositoIdIdDepositoNavigation { get; set; } = null!;

    public virtual FallaAuxilioVial FallaAuxilioIdIdfallaNavigation { get; set; } = null!;

    public virtual MotivoAuxilioVial MotivoAuxilioIdIdmotivoNavigation { get; set; } = null!;

    public virtual ICollection<NotificacionesSiniestro> NotificacionesSiniestros { get; set; } = new List<NotificacionesSiniestro>();

    public virtual Vehiculo VehiculoIdvehiculoNavigation { get; set; } = null!;
}
