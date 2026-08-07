using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Vehiculo
{
    public int IdVehiculo { get; set; }

    public sbyte TipovehiculoIdtipovehiculo { get; set; }

    public string? Placas { get; set; }

    public string? Vin { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public int? EmpresaIdempresa { get; set; }

    public string? Numeropoliza { get; set; }

    public DateTime? Fechafinseguro { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string? Numeroeconomico { get; set; }

    public string? Numeromotor { get; set; }

    public int? IdMarca { get; set; }

    public int? IdModelo { get; set; }

    public virtual ICollection<CombCarga> CombCargas { get; set; } = new List<CombCarga>();

    public virtual ICollection<CorridaAsignacion> CorridaAsignacions { get; set; } = new List<CorridaAsignacion>();

    public virtual ICollection<DetPrevMo> DetPrevMos { get; set; } = new List<DetPrevMo>();

    public virtual Empresa? EmpresaIdempresaNavigation { get; set; }

    public virtual ICollection<LlantaAsignacion> LlantaAsignacions { get; set; } = new List<LlantaAsignacion>();

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<RendMe> RendMes { get; set; } = new List<RendMe>();

    public virtual ICollection<Siniestro> Siniestros { get; set; } = new List<Siniestro>();

    public virtual ICollection<TarCom> TarComs { get; set; } = new List<TarCom>();

    public virtual Tipovehiculo TipovehiculoIdtipovehiculoNavigation { get; set; } = null!;

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();

    public virtual ICollection<VehiculoEje> VehiculoEjes { get; set; } = new List<VehiculoEje>();

    public virtual ICollection<Vehiculoservicio> Vehiculoservicios { get; set; } = new List<Vehiculoservicio>();
}
