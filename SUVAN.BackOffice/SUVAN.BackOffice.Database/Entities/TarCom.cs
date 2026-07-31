using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TarCom
{
    public string IdTarjet { get; set; } = null!;

    public int IdRegion { get; set; }

    public int IdPlanta { get; set; }

    public int IdZona { get; set; }

    public int IdDeposito { get; set; }

    public int IdVehiculo { get; set; }

    public int IdComb { get; set; }

    public int Idempresa { get; set; }

    public int IdDepto { get; set; }

    public string IdProv { get; set; } = null!;

    public DateTime FAlta { get; set; }

    public DateTime? FBaja { get; set; }

    public short? MotivoB { get; set; }

    public string Estatus { get; set; } = null!;

    public short CarMax { get; set; }

    public float TotCarg { get; set; }

    public double CosTar { get; set; }

    public short Color { get; set; }

    public int? Idusuario { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual Region Id { get; set; } = null!;

    public virtual TipoCom IdCombNavigation { get; set; } = null!;

    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    public virtual Depto IdDeptoNavigation { get; set; } = null!;

    public virtual Plantum IdPlantaNavigation { get; set; } = null!;

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;

    public virtual Zona IdZonaNavigation { get; set; } = null!;

    public virtual Empresa IdempresaNavigation { get; set; } = null!;

    public virtual Usuario? IdusuarioNavigation { get; set; }

    public virtual ICollection<TarIave> TarIaves { get; set; } = new List<TarIave>();
}
