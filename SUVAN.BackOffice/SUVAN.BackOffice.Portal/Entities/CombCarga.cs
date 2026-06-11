using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class CombCarga
{
    public int IdCarga { get; set; }

    public int IdVehiculo { get; set; }

    public DateOnly Fecha { get; set; }

    public TimeOnly Hora { get; set; }

    public string FolioNota { get; set; } = null!;

    public int Idfactura { get; set; }

    public double Importe { get; set; }

    public double Litros { get; set; }

    public double KmAnterior { get; set; }

    public double KmActual { get; set; }

    public double KmRecorridos { get; set; }

    public float Rendimiento { get; set; }

    public double CostoXLt { get; set; }

    public int IdComb { get; set; }

    public sbyte Traspasar { get; set; }

    public string Espec { get; set; } = null!;

    public DateTime FRegistro { get; set; }

    public int IdRegion { get; set; }

    public int IdPlanta { get; set; }

    public int IdZona { get; set; }

    public int IdDeposito { get; set; }

    public int Idempresa { get; set; }

    public int IdDepto { get; set; }

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

    public virtual Factura IdfacturaNavigation { get; set; } = null!;

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
