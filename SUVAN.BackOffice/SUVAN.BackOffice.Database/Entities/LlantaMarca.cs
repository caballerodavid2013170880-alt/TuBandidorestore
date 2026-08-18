using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaMarca
{
    public uint IdMarcaLlanta { get; set; }

    public string Nombre { get; set; } = null!;

    public bool? Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public uint CreadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public uint? ModificadoPor { get; set; }

    public virtual ICollection<LlantaModelo> LlantaModelos { get; set; } = new List<LlantaModelo>();
}
