using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class Insumo
{
    public int IdInsumo { get; set; }

    /// <summary>
    /// SKU o número de parte opcional
    /// </summary>
    public string? CodigoPieza { get; set; }

    /// <summary>
    /// Nombre de la refacción
    /// </summary>
    public string Descripcion { get; set; } = null!;

    /// <summary>
    /// Costo base sin impuestos
    /// </summary>
    public decimal CostoUnitario { get; set; }

    /// <summary>
    /// Monto del IVA aplicado
    /// </summary>
    public decimal Iva { get; set; }

    /// <summary>
    /// Suma de costo_unitario + iva
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
