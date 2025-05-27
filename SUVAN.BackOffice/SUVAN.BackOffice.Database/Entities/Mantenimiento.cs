using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Mantenimiento
{
    public int Idmantenimiento { get; set; }

    public int DepositoIddeposito { get; set; }

    public int MecanicoIdmecanico { get; set; }

    public int VehiculoIdvehiculo { get; set; }

    public string DepositoVehiculo { get; set; } = null!;

    public int TiposervicioIdtiposervicio { get; set; }

    public int PreventivoIdpreventivo { get; set; }

    public int CausamantenimientoIdcausamantenimiento { get; set; }

    public string Tanque { get; set; } = null!;

    public string KmsEntrada { get; set; } = null!;

    public string FechaProgramada { get; set; } = null!;

    public string FechaCaptura { get; set; } = null!;

    public string DesasignaVehiculo { get; set; } = null!;

    public string Observaciones { get; set; } = null!;

    public int MantenimientodetalleIdmantenimientodetalle { get; set; }

    public virtual CausaMantenimiento CausamantenimientoIdcausamantenimientoNavigation { get; set; } = null!;

    public virtual Depositosdisponible DepositoIddepositoNavigation { get; set; } = null!;

    public virtual MantenimientoDetalle MantenimientodetalleIdmantenimientodetalleNavigation { get; set; } = null!;

    public virtual Mecanico MecanicoIdmecanicoNavigation { get; set; } = null!;

    public virtual Preventivo PreventivoIdpreventivoNavigation { get; set; } = null!;

    public virtual TipoServicio TiposervicioIdtiposervicioNavigation { get; set; } = null!;

    public virtual Vehiculo VehiculoIdvehiculoNavigation { get; set; } = null!;
}
