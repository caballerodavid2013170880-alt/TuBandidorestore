using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Permiso
{
    public int PerfilIdperfil { get; set; }

    public int MenuIdmenu { get; set; }

    public ulong? Agregar { get; set; }

    public ulong? Modificar { get; set; }

    public ulong? Eliminar { get; set; }

    public ulong? Ejecutar { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual Menu MenuIdmenuNavigation { get; set; } = null!;

    public virtual Perfil PerfilIdperfilNavigation { get; set; } = null!;
}
