using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Corridum
{
    public int Idcorrida { get; set; }

    public int RutaIdruta { get; set; }

    public TimeOnly? HoraInicio { get; set; }

    public TimeOnly? HoraFin { get; set; }

    public int? EmpresaIdempresa { get; set; }

    public virtual ICollection<CorridaAsignacion> CorridaAsignacions { get; set; } = new List<CorridaAsignacion>();

    public virtual ICollection<CorridaDia> CorridaDia { get; set; } = new List<CorridaDia>();

    public virtual ICollection<CorridaParadum> CorridaParada { get; set; } = new List<CorridaParadum>();

    public virtual Empresa? EmpresaIdempresaNavigation { get; set; }

    public virtual ICollection<PromocionCorridum> PromocionCorrida { get; set; } = new List<PromocionCorridum>();

    public virtual Rutum RutaIdrutaNavigation { get; set; } = null!;
}
