using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Preventivo
{
    public int Idpreventivo { get; set; }

    public int? Idempresa { get; set; }

    public int? IdRegion { get; set; }

    public int? IdPlanta { get; set; }

    public int? IdZona { get; set; }

    public int? IdDeposito { get; set; }

    public string? NombrePreventivo { get; set; }

    public string? ObservacionesPreventivo { get; set; }

    public short? IdMarca { get; set; }

    public int IdModelo { get; set; }

    public DateTime FechaPrev { get; set; }

    public decimal? CostoTotal { get; set; }

    public int? Idusuario { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual ICollection<DetPrevMo> DetPrevMos { get; set; } = new List<DetPrevMo>();

    public virtual ICollection<DetPrev> DetPrevs { get; set; } = new List<DetPrev>();

    public virtual Region? Id { get; set; }

    public virtual Deposito? IdDepositoNavigation { get; set; }

    public virtual Marca? IdMarcaNavigation { get; set; }

    public virtual Modelo IdModeloNavigation { get; set; } = null!;

    public virtual Plantum? IdPlantaNavigation { get; set; }

    public virtual Zona? IdZonaNavigation { get; set; }

    public virtual Empresa? IdempresaNavigation { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
