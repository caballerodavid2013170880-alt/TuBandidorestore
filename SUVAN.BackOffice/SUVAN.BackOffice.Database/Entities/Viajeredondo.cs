using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Viajeredondo
{
    public int Idviajeredondo { get; set; }

    public string? Origennombre { get; set; }

    public decimal? Origenlatitud { get; set; }

    public decimal? Origenlongitud { get; set; }

    public string? Origendireccion { get; set; }

    public string? Destinonombre { get; set; }

    public decimal? Destinolatitud { get; set; }

    public decimal? Destinolongitud { get; set; }

    public string? Destinodireccion { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
}
