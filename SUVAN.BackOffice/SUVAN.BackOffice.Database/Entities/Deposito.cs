using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Deposito
{
    public short IdRegion { get; set; }

    public short IdPlanta { get; set; }

    public short IdZona { get; set; }

    public short IdDeposi { get; set; }

    public string Descrip { get; set; } = null!;

    public string? Direc { get; set; }

    public string? Ciudad { get; set; }

    public string? Respon { get; set; }

    public string? Tel { get; set; }

    public string? LocFor { get; set; }

    public string? RPerson { get; set; }

    public short? IdEmpresa { get; set; }

    public string? DescCorta { get; set; }

    public string? Rfc { get; set; }

    public string? Cp { get; set; }
}
