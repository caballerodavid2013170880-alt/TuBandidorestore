using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Modelo
{
    public short CMarca { get; set; }

    public short CModelo { get; set; }

    public short? ADesde { get; set; }

    public short? AHasta { get; set; }

    public short? CTipoV { get; set; }

    public string? Descrip { get; set; }

    public float? KmGaran { get; set; }

    public short? MesGara { get; set; }

    public short? TipoEje { get; set; }
}
