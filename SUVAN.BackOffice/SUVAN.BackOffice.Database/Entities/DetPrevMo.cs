using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class DetPrevMo
{
    public int IdPrevMo { get; set; }

    public int Idpreventivo { get; set; }

    public int IdVehiculo { get; set; }

    public int IdManoObra { get; set; }

    public decimal? Iva { get; set; }

    public decimal CostoTotalUnitario { get; set; }

    public int? Idusuario { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual ManoObra IdManoObraNavigation { get; set; } = null!;

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;

    public virtual Preventivo IdpreventivoNavigation { get; set; } = null!;
}
