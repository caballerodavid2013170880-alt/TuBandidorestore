using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Conversacionconexion
{
    public int ConversacionConexionId { get; set; }

    public int ConversacionId { get; set; }

    public string? ConexionId { get; set; }

    public string? TokenAcceso { get; set; }

    public int? EstatusConexion { get; set; }

    public DateTime? FechaHoraCreacion { get; set; }

    public virtual Conversacionportal Conversacion { get; set; } = null!;
}
