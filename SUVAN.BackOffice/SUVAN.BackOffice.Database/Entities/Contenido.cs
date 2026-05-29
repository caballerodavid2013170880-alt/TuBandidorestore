using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Contenido
{
    public int Idcontenido { get; set; }

    public int TipocontenidoIdtipocontenido { get; set; }

    public string? Titulo { get; set; }

    public string? Html { get; set; }

    public string? Imagen { get; set; }

    public int? Orden { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual Tipocontenido TipocontenidoIdtipocontenidoNavigation { get; set; } = null!;
}
