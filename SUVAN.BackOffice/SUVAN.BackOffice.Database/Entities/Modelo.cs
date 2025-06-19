using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Modelo
{
    public int AnioDesde { get; set; }

    public int AnioHasta { get; set; }

    public string Descripcion { get; set; } = null!;

    public float KmGarantia { get; set; }

    public int MesGarantia { get; set; }

    public int? TipoEje { get; set; }

    public short IdMarca { get; set; }

    public sbyte IdTipoV { get; set; }

    public int IdModelo { get; set; }

    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    public virtual Tipovehiculo IdTipoVNavigation { get; set; } = null!;

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();
}
