using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Preventivo
{
    public int Idpreventivo { get; set; }

    public string? NombrePreventivo { get; set; }

    public string? ObservacionesPreventivo { get; set; }

    public int IdPlanta { get; set; }

    public string? Meses { get; set; }

    public short? IdMarca { get; set; }

    public int IdModelo { get; set; }

    public int IdRegion { get; set; }

    public int IdZona { get; set; }

    public int IdDeposito { get; set; }

    public int IdDeptos { get; set; }

    public DateTime? FechaPrev { get; set; }

    public int Idempresa { get; set; }

    public int? Idusuario { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual ICollection<DetPrev> DetPrevs { get; set; } = new List<DetPrev>();

    public virtual Region Id { get; set; } = null!;

    public virtual Marca? IdMarcaNavigation { get; set; }

    public virtual Modelo IdModeloNavigation { get; set; } = null!;

    public virtual Empresa IdempresaNavigation { get; set; } = null!;

    public virtual Usuario? IdusuarioNavigation { get; set; }

}
