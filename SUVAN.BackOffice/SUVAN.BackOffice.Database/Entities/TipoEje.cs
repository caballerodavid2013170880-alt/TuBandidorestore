using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class TipoEje
{
    public int IdTipoEje { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public byte NumeroPosiciones { get; set; }

    public bool? EsActivo { get; set; }

    public ushort NumeroEje { get; set; }

    public virtual ICollection<LlantaAsignacion> LlantaAsignacions { get; set; } = new List<LlantaAsignacion>();

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();
}
