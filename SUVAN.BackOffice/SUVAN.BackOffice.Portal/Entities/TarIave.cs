using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class TarIave
{
    public string IdIave { get; set; } = null!;

    public int Idempresa { get; set; }

    public int IdRegion { get; set; }

    public int IdPlanta { get; set; }

    public int IdZona { get; set; }

    public int IdDeposito { get; set; }

    public int IdDepto { get; set; }

    public string IdTarjet { get; set; } = null!;

    public string IdProv { get; set; } = null!;

    public DateTime FAlta { get; set; }

    public DateTime FBaja { get; set; }

    public short MotivoB { get; set; }

    public string Estatus { get; set; } = null!;

    public short Color { get; set; }

    public int? Idusuario { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public int? Idconductor { get; set; }

    public virtual Region Id { get; set; } = null!;

    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    public virtual Depto IdDeptoNavigation { get; set; } = null!;

    public virtual Plantum IdPlantaNavigation { get; set; } = null!;

    public virtual TarCom IdTarjetNavigation { get; set; } = null!;

    public virtual Zona IdZonaNavigation { get; set; } = null!;

    public virtual Conductor? IdconductorNavigation { get; set; }

    public virtual Empresa IdempresaNavigation { get; set; } = null!;

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
