using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Conversacionusuario
{
    public int ConversacionUsuarioId { get; set; }

    public int ConversacionId { get; set; }

    public int UsuarioId { get; set; }

    public string? TipoUsuario { get; set; }

    public virtual Conversacionportal Conversacion { get; set; } = null!;
}
