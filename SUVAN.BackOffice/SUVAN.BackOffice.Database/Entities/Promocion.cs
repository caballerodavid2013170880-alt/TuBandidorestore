using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Promocion
{
    public int Idpromocion { get; set; }

    public string? Nombre { get; set; }

    public DateTime? Vigenciadesde { get; set; }

    public DateTime? Vigenciahasta { get; set; }

    public int TipopromocionIdtipopromocion { get; set; }

    public decimal? Cantidad { get; set; }

    public int TipodescuentoIdtipodescuento { get; set; }

    public int? EmpresaIdempresa { get; set; }

    public virtual Empresa? EmpresaIdempresaNavigation { get; set; }

    public virtual ICollection<PromocionCorridum> PromocionCorrida { get; set; } = new List<PromocionCorridum>();

    public virtual ICollection<PromocionEmpresa> PromocionEmpresas { get; set; } = new List<PromocionEmpresa>();

    public virtual ICollection<PromocionHorario> PromocionHorarios { get; set; } = new List<PromocionHorario>();

    public virtual ICollection<PromocionRutum> PromocionRuta { get; set; } = new List<PromocionRutum>();

    public virtual Tipodescuento TipodescuentoIdtipodescuentoNavigation { get; set; } = null!;

    public virtual Tipopromocion TipopromocionIdtipopromocionNavigation { get; set; } = null!;

    public virtual ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
}
