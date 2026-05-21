using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Portal.Entities;

public partial class DetPrev
{
    public int Idpreventivo { get; set; }

    public int IdPrevDet { get; set; }

    public int IdManoObra { get; set; }

    public virtual ManoObra IdManoObraNavigation { get; set; } = null!;

    public virtual Preventivo IdpreventivoNavigation { get; set; } = null!;
}
