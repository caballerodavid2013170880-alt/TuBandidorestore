using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Mensaje
{
    public int MensajeId { get; set; }

    public string? Comentario { get; set; }

    public int UsuarioId { get; set; }

    public int TipoUsuario { get; set; }

    public DateTime? FechaHoraCreacion { get; set; }

    public virtual ICollection<Conversacionmensaje> Conversacionmensajes { get; set; } = new List<Conversacionmensaje>();
}
