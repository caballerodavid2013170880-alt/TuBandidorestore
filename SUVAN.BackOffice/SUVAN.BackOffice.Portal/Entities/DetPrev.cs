using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class DetPrev
{
    public int Idpreventivo { get; set; }

    public int IdPrevDet { get; set; }

    public int IdManoObra { get; set; }

    /// <summary>
    /// FK al catálogo global de refacciones
    /// </summary>
    public int? IdInsumo { get; set; }

    /// <summary>
    /// Cantidad de piezas o horas laboradas
    /// </summary>
    public decimal Cantidad { get; set; }

    /// <summary>
    /// Costo unitario congelado al momento del servicio
    /// </summary>
    public decimal CostoUnitario { get; set; }

    /// <summary>
    /// Monto de IVA congelado
    /// </summary>
    public decimal Iva { get; set; }

    /// <summary>
    /// Total de la partida: (costo_unitario * cantidad) + iva
    /// </summary>
    public decimal CostoTotal { get; set; }

    public int? Idusuario { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual Insumo? IdInsumoNavigation { get; set; }

    public virtual ManoObra IdManoObraNavigation { get; set; } = null!;

    public virtual Preventivo IdpreventivoNavigation { get; set; } = null!;

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
