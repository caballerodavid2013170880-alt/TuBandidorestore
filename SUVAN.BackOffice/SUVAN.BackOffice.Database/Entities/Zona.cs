using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Zona
{
    public int IdZona { get; set; }

    public int IdRegion { get; set; }

    public int IdPlanta { get; set; }

    public string NombreZona { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public string Domicilio { get; set; } = null!;

    public string Telefono1 { get; set; } = null!;

    public string Telefono2 { get; set; } = null!;

    public string Responsable { get; set; } = null!;

    public DateTime FechaApertura { get; set; }

    public int IdEmpresa { get; set; }

    public ulong Activo { get; set; }

    public virtual ICollection<CombCarga> CombCargas { get; set; } = new List<CombCarga>();

    public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();

    public virtual ICollection<Depto> Deptos { get; set; } = new List<Depto>();

    public virtual Region Id { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Plantum IdPlantaNavigation { get; set; } = null!;

    public virtual ICollection<Preventivo> Preventivos { get; set; } = new List<Preventivo>();

    public virtual ICollection<RendMe> RendMes { get; set; } = new List<RendMe>();

    public virtual ICollection<Taller> Tallers { get; set; } = new List<Taller>();

    public virtual ICollection<TarCom> TarComs { get; set; } = new List<TarCom>();

    public virtual ICollection<TarIave> TarIaves { get; set; } = new List<TarIave>();

    public virtual ICollection<UsuarioJerarquium> UsuarioJerarquia { get; set; } = new List<UsuarioJerarquium>();

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();
}
