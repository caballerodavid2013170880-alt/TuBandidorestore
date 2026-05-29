using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Zona1
{
    public short IdRegion { get; set; }

    public short IdPlanta { get; set; }

    public short IdZona { get; set; }

    public string? Rfc { get; set; }

    public string? Nombre { get; set; }

    public string? Domicil { get; set; }

    public string? Tel1 { get; set; }

    public string? Tel2 { get; set; }

    public string? Fax { get; set; }

    public string? Respon { get; set; }

    public DateTime? FAper { get; set; }

    public bool? SucXOm { get; set; }

    public string? SerFac { get; set; }

    public string? SerNc { get; set; }

    public bool? TieFtal { get; set; }

    public short? AviAlar { get; set; }

    public bool? Presalar { get; set; }

    public bool? DesTemp { get; set; }

    public short? CtaMo { get; set; }

    public short? IdEmpresa { get; set; }
}
