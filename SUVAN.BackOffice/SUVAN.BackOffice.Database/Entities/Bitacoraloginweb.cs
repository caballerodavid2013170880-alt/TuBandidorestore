using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Bitacoraloginweb
{
    public int Id { get; set; }

    public int? Idusuario { get; set; }

    public string? Email { get; set; }

    public DateTime? Fechaaccion { get; set; }

    public string? Detalle { get; set; }

    public string? Codigo { get; set; }

    public DateTime? Fechaexpiracodigo { get; set; }

    public string? Error { get; set; }
}
