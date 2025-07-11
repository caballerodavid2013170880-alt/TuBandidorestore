using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class VehiculoDetalle
{
    public int IdVehiculoDetalle { get; set; }

    public int IdVehiculo { get; set; }

    public sbyte IdTipoVehiculo { get; set; }

    public short IdMarca { get; set; }

    public int IdZona { get; set; }

    public int IdDeposito { get; set; }

    public int? IdEspecificacion { get; set; }

    public int IdModelo { get; set; }

    public int IdTipoEje { get; set; }

    public int? IdNegocio { get; set; }

    public int AnioVehiculo { get; set; }

    public string ColorVehiculo { get; set; } = null!;

    public ulong? TieneRotulo { get; set; }

    public string NumeroSerie { get; set; } = null!;

    public string NumeroMotor { get; set; } = null!;

    public string Carroceria { get; set; } = null!;

    public string TarjetaCirculacion { get; set; } = null!;

    public int Gasolina { get; set; }

    public ulong? CopiaFactura { get; set; }

    public ulong? CopiaTarjetaCir { get; set; }

    public ulong? CopiaPlaca { get; set; }

    public ulong? CopiaVerificacion { get; set; }

    public ulong? CopiaPolizaSeguro { get; set; }

    public string Proveedor { get; set; } = null!;

    public DateTime? FechaCompra { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public float CostoVehiculo { get; set; }

    public ulong? TarifaVehicular { get; set; }

    public string? NombreTarifaVehicular { get; set; }

    public float KilometrajeAcumulado { get; set; }

    public DateTime? FechaBaja { get; set; }

    public string ColorInterior { get; set; } = null!;

    public ulong? TieneCaja { get; set; }

    public ulong? NecesitaRemolque { get; set; }

    public ulong? VehiculoRelevo { get; set; }

    public ulong? Rentado { get; set; }

    public float KilometrajeGarantia { get; set; }

    public int MesesGarantia { get; set; }

    public string? EconomicoAnterior { get; set; }

    public int PesoMinimo { get; set; }

    public int PesoMaximo { get; set; }

    public float VolumenMinimo { get; set; }

    public float VolumenMaximo { get; set; }

    public string TipoLicenciaRequerida { get; set; } = null!;

    public ulong? PermisoCargaAceite { get; set; }

    public DateTime VigenciaPermisoAceite { get; set; }

    public DateTime? VigenciaTarjetaCirculacion { get; set; }

    public string UsuarioCaptura { get; set; } = null!;

    public virtual Depositosdisponible IdDepositoNavigation { get; set; } = null!;

    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    public virtual Modelo IdModeloNavigation { get; set; } = null!;

    public virtual TipoEje IdTipoEjeNavigation { get; set; } = null!;

    public virtual Tipovehiculo IdTipoVehiculoNavigation { get; set; } = null!;

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;

    public virtual Zona IdZonaNavigation { get; set; } = null!;
}
