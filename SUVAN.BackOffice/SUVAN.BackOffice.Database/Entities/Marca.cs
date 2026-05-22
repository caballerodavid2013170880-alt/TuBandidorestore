using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Marca
{
    public short IdMarca { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();

    public virtual ICollection<Preventivo> Preventivos { get; set; } = new List<Preventivo>();

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();
}
