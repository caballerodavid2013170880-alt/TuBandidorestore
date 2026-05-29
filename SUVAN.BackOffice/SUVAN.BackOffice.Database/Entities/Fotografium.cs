using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Fotografium
{
    public int UsuarioIdusuario { get; set; }

    public string? Imagen { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;
}
