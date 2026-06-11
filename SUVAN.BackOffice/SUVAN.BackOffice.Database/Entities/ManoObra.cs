using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class ManoObra
{
    public int IdManoObra { get; set; }

    public string? DescripcionManoobra { get; set; }

    /// <summary>
    /// Monto del IVA aplicado
    /// </summary>
    public decimal Iva { get; set; }

    /// <summary>
    /// Costo hora/servicio con impuestos
    /// </summary>
    public decimal CostoTotal { get; set; }

    /// <summary>
    /// Bandera de baja lógica
    /// </summary>
    public bool? Activo { get; set; }

    /// <summary>
    /// Usuario que creó/modificó (FK)
    /// </summary>
    public int? Idusuario { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual ICollection<DetPrev> DetPrevs { get; set; } = new List<DetPrev>();

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
