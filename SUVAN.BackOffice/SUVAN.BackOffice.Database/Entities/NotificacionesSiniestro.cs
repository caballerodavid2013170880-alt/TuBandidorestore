using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class NotificacionesSiniestro
{
    public uint IdNotifsini { get; set; }

    public int SiniestroIdsiniestro { get; set; }

    public string? TipoNotificacion { get; set; }

    public string? MedioContacto { get; set; }

    public string DestinatarioNombre { get; set; } = null!;

    public string Asunto { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public DateTime FechaEnvio { get; set; }

    public string EnviadoPor { get; set; } = null!;

    public virtual Siniestro SiniestroIdsiniestroNavigation { get; set; } = null!;
}
