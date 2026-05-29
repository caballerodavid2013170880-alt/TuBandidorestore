using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Conversacionportal
{
    public int ConversacionId { get; set; }

    /// <summary>
    /// 0=Empresa
    /// 1=Ruta
    /// 2=Operador
    /// </summary>
    public int TipoConversacion { get; set; }

    public int? EmpresaId { get; set; }

    public int? Ruta { get; set; }

    public int? Operador { get; set; }

    public int UsuarioCreacion { get; set; }

    public string Titulo { get; set; } = null!;

    /// <summary>
    /// 1=Activo
    /// 0=Cerrada
    /// </summary>
    public ulong EstatusConversacion { get; set; }

    public DateTime FechaHoraCreacion { get; set; }

    public virtual ICollection<Conversacionconexion> Conversacionconexions { get; set; } = new List<Conversacionconexion>();

    public virtual ICollection<Conversacionmensaje> Conversacionmensajes { get; set; } = new List<Conversacionmensaje>();

    public virtual ICollection<Conversacionusuario> Conversacionusuarios { get; set; } = new List<Conversacionusuario>();
}
