using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaEstado
{
    public ushort IdEstadoLlanta { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public virtual ICollection<Llantum> Llanta { get; set; } = new List<Llantum>();
}
