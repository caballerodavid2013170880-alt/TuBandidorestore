using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Preventivo
{
    public int Idpreventivo { get; set; }

    public string Nombre { get; set; } = null!;

    public string Proyecta { get; set; } = null!;

    public int TiposervicioIdtiposervicio { get; set; }

    public virtual TipoServicio TiposervicioIdtiposervicioNavigation { get; set; } = null!;
}
