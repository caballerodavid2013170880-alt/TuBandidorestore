using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Menu
{
    public int Idmenu { get; set; }

    public int? MenuIdpadre { get; set; }

    public string? Titulo { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public string? Icono { get; set; }

    public string? Ruta { get; set; }

    public virtual ICollection<Menu> InverseMenuIdpadreNavigation { get; set; } = new List<Menu>();

    public virtual Menu? MenuIdpadreNavigation { get; set; }

    public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
}
