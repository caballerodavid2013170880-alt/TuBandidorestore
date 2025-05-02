using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Vehiculoservicio
{
    public int Idvehiculoservicio { get; set; }

    public int Idvehiculo { get; set; }

    public string? Detalle { get; set; }

    public DateTime? Fechaservicio { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual Vehiculo IdvehiculoNavigation { get; set; } = null!;
}
