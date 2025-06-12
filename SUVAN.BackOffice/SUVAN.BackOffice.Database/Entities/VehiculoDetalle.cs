using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class VehiculoDetalle
{
    public int IdVehicDetalle { get; set; }

    public string? IdVehiculo { get; set; }

    public int? IdTipovehi { get; set; }

    public int? IdMarca { get; set; }

    public int? IdZona { get; set; }

    public int? IdDeposito { get; set; }

    public int? IdEspeci { get; set; }

    public int? IdModelo { get; set; }

    public int? IdPermisoAceite { get; set; }

    public int? IdCognos { get; set; }

    public int? IdTipoEje { get; set; }

    public int? Negocio { get; set; }

    public int? Area { get; set; }

    public int? Anio { get; set; }

    public string? Color { get; set; }

    public ulong? Rotulo { get; set; }

    public string? PlacaPe { get; set; }

    public string? Serie { get; set; }

    public string? Motor { get; set; }

    public string? Carroc { get; set; }

    public string? TarCirc { get; set; }

    public int? Gasoline { get; set; }

    public int? Encierro { get; set; }

    public ulong? CopFac { get; set; }

    public ulong? CopTcir { get; set; }

    public ulong? CopPla { get; set; }

    public ulong? CopVer { get; set; }

    public ulong? CopPol { get; set; }

    public string? NoCirc { get; set; }

    public string? DnoCirc { get; set; }

    public string? Proveed { get; set; }

    public DateTime? FCompra { get; set; }

    public string? Factura { get; set; }

    public float? Costo { get; set; }

    public ulong? Tariave { get; set; }

    public string? Ntariave { get; set; }

    public float? KmAcum { get; set; }

    public string? StVehic { get; set; }

    public DateTime? FBaja { get; set; }

    public string? ColInt { get; set; }

    public float? RegFed { get; set; }

    public int? EdregPl { get; set; }

    public int? ColEst { get; set; }

    public ulong? Caja { get; set; }

    public ulong? NecRem { get; set; }

    public ulong? Relevo { get; set; }

    public ulong? Rentado { get; set; }

    public int? ColRuta { get; set; }

    public int? CauBaja { get; set; }

    public float? VRecupe { get; set; }

    public float? KmGaran { get; set; }

    public int? MesGara { get; set; }

    public string? EcoAnt { get; set; }

    public ulong? LocFor { get; set; }

    public int? Baja { get; set; }

    public int? PesoMinimo { get; set; }

    public int? PesoMaximo { get; set; }

    public decimal? VolumenMinimo { get; set; }

    public decimal? VolumenMaximo { get; set; }

    public string? TipoLicenciaRequerida { get; set; }

    public ulong? PermisoCargaAceite { get; set; }

    public DateTime? VigenciaPermisoAceite { get; set; }

    public DateTime? VigenciaTarjetaCircula { get; set; }

    public string? Asigna { get; set; }

    public int? Total { get; set; }

    public string? Usuario { get; set; }
}
