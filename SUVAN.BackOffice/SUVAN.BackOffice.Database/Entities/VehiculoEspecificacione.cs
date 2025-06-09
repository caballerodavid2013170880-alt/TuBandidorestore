using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class VehiculoEspecificacione
{
    public short IdEspeci { get; set; }

    public short IdModVe { get; set; }

    public short IdMarca { get; set; }

    public short? TipoCom { get; set; }

    public short? Pallets { get; set; }

    public double? RenEsp { get; set; }

    public double? CapCom { get; set; }

    public double? CapAce { get; set; }

    public string? TipoMot { get; set; }

    public string? PotMot { get; set; }

    public string? PulCub { get; set; }

    public short? NoCilin { get; set; }

    public double? Ancho { get; set; }

    public double? Largo { get; set; }

    public double? Altura { get; set; }

    public float? PesoBru { get; set; }

    public float? CarEje { get; set; }

    public float? CarMax { get; set; }

    public short? LlanRep { get; set; }

    public short? TotLlan { get; set; }

    public short? DimLlan { get; set; }

    public string? Transm { get; set; }

    public string? Traccion { get; set; }

    public string? Origen { get; set; }

    public short? TipoEje { get; set; }

    public double? McubCar { get; set; }

    public double? TonCar { get; set; }

    public string? Obs { get; set; }
}
