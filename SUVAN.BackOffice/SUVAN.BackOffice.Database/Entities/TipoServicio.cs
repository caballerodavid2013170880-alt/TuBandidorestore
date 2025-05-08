using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TipoServicio
{
    public int IdTiposervicio { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<Preventivo> Preventivos { get; set; } = new List<Preventivo>();
}
