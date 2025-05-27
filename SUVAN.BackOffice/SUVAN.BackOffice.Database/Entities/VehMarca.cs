using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class VehMarca
{
    public short Marca { get; set; }

    public short Modelo { get; set; }

    public short Especif { get; set; }

    public string Asigna { get; set; } = null!;

    public int? Total { get; set; }

    public string Usuario { get; set; } = null!;
}
