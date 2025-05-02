using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Perfil
{
    public int Idperfil { get; set; }

    public string? Nombre { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public virtual ICollection<AdminEmpresa> AdminEmpresas { get; set; } = new List<AdminEmpresa>();

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
}
