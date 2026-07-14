using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Plantum
{
    public int IdRegion { get; set; }

    public int IdPlanta { get; set; }

    public string? NombrePlanta { get; set; }

    public string? Libreria { get; set; }

    public int IdEmpresa { get; set; }

    public ulong? Activo { get; set; }

    public virtual ICollection<CombCarga> CombCargas { get; set; } = new List<CombCarga>();

    public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();

    public virtual ICollection<Depto> Deptos { get; set; } = new List<Depto>();

    public virtual Region Id { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual ICollection<Preventivo> Preventivos { get; set; } = new List<Preventivo>();

    public virtual ICollection<RendMe> RendMes { get; set; } = new List<RendMe>();

    public virtual ICollection<TarCom> TarComs { get; set; } = new List<TarCom>();

    public virtual ICollection<TarIave> TarIaves { get; set; } = new List<TarIave>();

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();

    public virtual ICollection<Zona> Zonas { get; set; } = new List<Zona>();
}
