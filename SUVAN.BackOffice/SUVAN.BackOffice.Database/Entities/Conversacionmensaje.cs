using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Conversacionmensaje
{
    public int ConversacionMensajeId { get; set; }

    public int ConversacionId { get; set; }

    public int MensajeId { get; set; }

    public int UsuarioId { get; set; }

    /// <summary>
    /// 0=No Enviado
    /// 1=Enviado
    /// -1=Error
    /// </summary>
    public int Estatus { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public ulong? Error { get; set; }

    public string? MensajeError { get; set; }

    public virtual Conversacionportal Conversacion { get; set; } = null!;

    public virtual Mensaje Mensaje { get; set; } = null!;
}
