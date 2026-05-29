using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Tokenpago
{
    public int Idtokenpago { get; set; }

    public string? Token { get; set; }

    public string? TokenRefresh { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public int UsuarioIdusuario { get; set; }

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;
}
