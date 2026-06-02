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

    public string? DescCorta { get; set; }

    public string? Rfc { get; set; }

    public string? Cp { get; set; }

    //agregado activo manualmente para el borrado lógico
    public ulong? Activo { get; set; }

    public virtual ICollection<CombCarga> CombCargas { get; set; } = new List<CombCarga>();

    public virtual ICollection<Depto> Deptos { get; set; } = new List<Depto>();

    public virtual Region Id { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Plantum IdPlantaNavigation { get; set; } = null!;

    public virtual ICollection<RendMe> RendMes { get; set; } = new List<RendMe>();

    public virtual ICollection<TarCom> TarComs { get; set; } = new List<TarCom>();

    public virtual ICollection<TarIave> TarIaves { get; set; } = new List<TarIave>();
}
