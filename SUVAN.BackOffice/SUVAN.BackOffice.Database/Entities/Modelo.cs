using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Modelo
{
    public short IdMarca { get; set; }

    public short IdModelo { get; set; }

    public short? ADesde { get; set; }

    public short? AHasta { get; set; }

    public short? IdTipoV { get; set; }

    public string? Descrip { get; set; }

    public float? KmGaran { get; set; }

    public short? MesGara { get; set; }

    public short? TipoEje { get; set; }
}
