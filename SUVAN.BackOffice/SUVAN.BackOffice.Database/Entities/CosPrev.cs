using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CosPrev
{
    public short IdRegion { get; set; }

    public short IdPlanta { get; set; }

    public short IdMarca { get; set; }

    public short IdModelo { get; set; }

    public short IdPrev { get; set; }

    public float? CosMo { get; set; }

    public float? CosRef { get; set; }

    public float? CosTot { get; set; }
}
