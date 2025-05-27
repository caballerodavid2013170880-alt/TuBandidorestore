using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class MantenimientoDet
{
    public int IdMantenimientoDet { get; set; }

    public int IdMantenimiento { get; set; }

    public int Renglon { get; set; }

    public short IdTipoReparacion { get; set; }

    public short? Cantidad { get; set; }

    public string? Descripcion { get; set; }

    public float? Precio { get; set; }

    public string? TiempoEmpleado { get; set; }

    public float? ValTall { get; set; }

    public DateTime? FechaProgramada { get; set; }

    public virtual TipoReparacion IdTipoReparacionNavigation { get; set; } = null!;
}
