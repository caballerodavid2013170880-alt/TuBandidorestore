using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class AdminEmpresa
{
    public int AdminIdadmin { get; set; }

    public int EmpresaIdempresa { get; set; }

    public int PerfilIdperfil { get; set; }

    public ulong? Principal { get; set; }

    public virtual Admin AdminIdadminNavigation { get; set; } = null!;

    public virtual Empresa EmpresaIdempresaNavigation { get; set; } = null!;

    public virtual Perfil PerfilIdperfilNavigation { get; set; } = null!;
}
