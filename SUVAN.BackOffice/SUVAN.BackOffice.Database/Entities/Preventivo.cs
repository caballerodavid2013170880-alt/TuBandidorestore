using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Preventivo
{
    public int Idpreventivo { get; set; }

    public string Nombre { get; set; } = null!;

    public string Proyecta { get; set; } = null!;

    public int TiposervicioIdtiposervicio { get; set; }

    public string? ObservacionesPreventivo { get; set; }

    public short IdPlanta { get; set; }

    public string? Meses { get; set; }

    public short? IdMarca { get; set; }

    public short? IdModelo { get; set; }

    public short IdRegion { get; set; }

    public short IdZona { get; set; }

    public short IdDeposito { get; set; }

    public short IdDeptos { get; set; }

    public DateTime? FechaPrev { get; set; }

    public virtual TipoServicio TiposervicioIdtiposervicioNavigation { get; set; } = null!;
}
