using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Paradum
{
    public int Idparada { get; set; }

    public string? Nombre { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public int? Orden { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong Activo { get; set; }

    public string? Calle { get; set; }

    public string? Numero { get; set; }

    public string? Municipio { get; set; }

    public string? Ciudad { get; set; }

    public string? Codigopostal { get; set; }

    public string? Colonia { get; set; }

    public virtual ICollection<CorridaParadum> CorridaParada { get; set; } = new List<CorridaParadum>();

    public virtual ICollection<RutaParadum> RutaParada { get; set; } = new List<RutaParadum>();

    public virtual ICollection<TarifaEscalonadum> TarifaEscalonadumParadaIdparada1Navigations { get; set; } = new List<TarifaEscalonadum>();

    public virtual ICollection<TarifaEscalonadum> TarifaEscalonadumParadaIdparadaNavigations { get; set; } = new List<TarifaEscalonadum>();

    public virtual ICollection<Viaje> ViajeParadaFinNavigations { get; set; } = new List<Viaje>();

    public virtual ICollection<Viaje> ViajeParadaInicioNavigations { get; set; } = new List<Viaje>();
}
