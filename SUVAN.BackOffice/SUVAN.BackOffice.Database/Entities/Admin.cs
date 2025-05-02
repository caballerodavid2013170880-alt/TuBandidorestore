using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Admin
{
    public int Idadmin { get; set; }

    public string? Email { get; set; }

    public string? Nombre { get; set; }

    public string? Hashpassword { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public int? PerfilIdperfil { get; set; }

    public string? FirebaseId { get; set; }

    public virtual ICollection<AdminEmpresa> AdminEmpresas { get; set; } = new List<AdminEmpresa>();

    public virtual Mfaportal? Mfaportal { get; set; }

    public virtual Perfil? PerfilIdperfilNavigation { get; set; }
}
