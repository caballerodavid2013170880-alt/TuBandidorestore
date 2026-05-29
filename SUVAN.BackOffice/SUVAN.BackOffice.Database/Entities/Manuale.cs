using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Manuale
{
    public short CMarca { get; set; }

    public short CModelo { get; set; }

    public short Consec { get; set; }

    public string? Nombre { get; set; }

    public byte[]? Archivo { get; set; }
}
