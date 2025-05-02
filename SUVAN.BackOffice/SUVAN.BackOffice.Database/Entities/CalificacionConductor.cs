using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CalificacionConductor
{
    public int ViajeIdviaje { get; set; }

    public int UsuarioIdusuario { get; set; }

    public int ConductorIdconductor { get; set; }

    public int? Calificacion { get; set; }

    public string? Mensaje { get; set; }

    public virtual Conductor ConductorIdconductorNavigation { get; set; } = null!;

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;

    public virtual Viaje ViajeIdviajeNavigation { get; set; } = null!;
}
