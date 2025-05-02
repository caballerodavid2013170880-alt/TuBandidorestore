using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Token
{
    public int Idtoken { get; set; }

    public string? Token1 { get; set; }

    public string? TokenRefresh { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public int UsuarioIdusuario { get; set; }

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;
}
