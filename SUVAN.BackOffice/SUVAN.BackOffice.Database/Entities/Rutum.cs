using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Rutum
{
    public int Idruta { get; set; }

    public string? Nombre { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public int? EmpresaIdempresa { get; set; }

    public int? TipotarifaIdtipotarifa { get; set; }

    public string? Googlemapsruta { get; set; }

    public int? Distanciamts { get; set; }

    public virtual ICollection<Corridum> Corrida { get; set; } = new List<Corridum>();

    public virtual Empresa? EmpresaIdempresaNavigation { get; set; }

    public virtual ICollection<PromocionRutum> PromocionRuta { get; set; } = new List<PromocionRutum>();

    public virtual ICollection<RutaParadum> RutaParada { get; set; } = new List<RutaParadum>();

    public virtual TarifaGeneral? TarifaGeneral { get; set; }

    public virtual Tipotarifa? TipotarifaIdtipotarifaNavigation { get; set; }
}
