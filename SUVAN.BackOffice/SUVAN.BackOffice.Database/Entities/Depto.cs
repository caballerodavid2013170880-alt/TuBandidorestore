using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Depto
{
    public int IdRegion { get; set; }

    public int IdPlanta { get; set; }

    public int IdZona { get; set; }

    public int IdDeposito { get; set; }

    public int IdDepto { get; set; }

    public string? NombreDepto { get; set; }

    public string Responsable { get; set; } = null!;

    public int IdEmpresa { get; set; }

    public ulong? Activo { get; set; }

    //DCC
    public virtual Deposito Id { get; set; } = null!;
    public virtual ICollection<CombCarga> CombCargas { get; set; } = new List<CombCarga>();
    public virtual ICollection<Region> Regiones { get; set; } = new List<Region>();
    public virtual ICollection<Plantum> Plantas { get; set; } = new List<Plantum>();
    public virtual ICollection<Zona> Zonas { get; set; } = new List<Zona>();
    public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();

    //public virtual Region Id { get; set; } = null!;

    //public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    //public virtual Plantum IdPlantaNavigation { get; set; } = null!;

    //public virtual Zona IdZonaNavigation { get; set; } = null!;
    
    public virtual ICollection<RendMe> RendMes { get; set; } = new List<RendMe>();

    public virtual ICollection<TarCom> TarComs { get; set; } = new List<TarCom>();

    public virtual ICollection<TarIave> TarIaves { get; set; } = new List<TarIave>();
}
