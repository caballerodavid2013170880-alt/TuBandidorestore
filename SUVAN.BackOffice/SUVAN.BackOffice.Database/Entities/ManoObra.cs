using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class ManoObra
{
    public int IdManoObra { get; set; }

    public string? DescripcionManoobra { get; set; }

    public decimal? CostoUnitario { get; set; }

    /// <summary>
    /// Usuario que creó/modificó (FK)
    /// </summary>
    public int? Idusuario { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual ICollection<DetPrevMo> DetPrevMos { get; set; } = new List<DetPrevMo>();

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
