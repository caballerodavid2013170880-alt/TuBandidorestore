using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Depto
{
    public short IdRegion { get; set; }

    public short IdPlanta { get; set; }

    public short IdZona { get; set; }

    public short IdDeposi { get; set; }

    public short IdDepto { get; set; }

    public string Descrip { get; set; } = null!;

    public string Responsable { get; set; } = null!;

}