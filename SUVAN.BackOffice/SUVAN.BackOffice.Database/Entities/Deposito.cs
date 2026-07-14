using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Deposito
{
    public int IdRegion { get; set; }

    public int IdPlanta { get; set; }

    public int IdZona { get; set; }

    public int IdDeposito { get; set; }

    public string? NombreDeposito { get; set; }

    public string? Direc { get; set; }

    public string? Ciudad { get; set; }

    public string? Respon { get; set; }

    public string? Tel { get; set; }

    public string? LocFor { get; set; }

    public string? RPerson { get; set; }

    public int IdEmpresa { get; set; }

    public string? NomCorto { get; set; }

    public string? Rfc { get; set; }

    public string? Cp { get; set; }

    public ulong? Activo { get; set; }
    //DCC
    public virtual Zona Id { get; set; } = null!;
    public virtual ICollection<CombCarga> CombCargas { get; set; } = new List<CombCarga>();

    //public virtual ICollection<Depto> Deptos { get; set; } = new List<Depto>();

    public virtual ICollection<Region> Regiones { get; set; } = new List<Region>();
    public virtual ICollection<Plantum> Plantas { get; set; } = new List<Plantum>();
    public virtual ICollection<Zona> Zonas { get; set; } = new List<Zona>();
    //public virtual Region Id { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    //public virtual Plantum IdPlantaNavigation { get; set; } = null!;

    public virtual ICollection<Preventivo> Preventivos { get; set; } = new List<Preventivo>();

    public virtual ICollection<RendMe> RendMes { get; set; } = new List<RendMe>();

    public virtual ICollection<TarCom> TarComs { get; set; } = new List<TarCom>();

    public virtual ICollection<TarIave> TarIaves { get; set; } = new List<TarIave>();
}
