using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class ManoObraDetalle
{
    public int IdMoDetalle { get; set; }

    public int IdManoObra { get; set; }

    public string DescripcionActividad { get; set; } = null!;

    public sbyte EsObligatorio { get; set; }

    public int Idusuario { get; set; }

    public DateTime Fecharegistro { get; set; }

    public virtual ManoObra IdManoObraNavigation { get; set; } = null!;
}
