using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Depto
{
    public short IdRegion { get; set; }

    public short IdPlanta { get; set; }

    public int IdZona { get; set; }

    public short IdDeposi { get; set; }

    public short IdDepto { get; set; }

    public string Descrip { get; set; } = null!;

    public string Responsable { get; set; } = null!;

    public virtual Region Region { get; set; }
    public virtual Plantum Plantum { get; set; }
    public virtual Zona Zona { get; set; }
    public virtual Deposito Deposito { get; set; }

}
