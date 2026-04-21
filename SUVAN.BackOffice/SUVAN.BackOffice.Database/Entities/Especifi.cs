using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Especifi
{
    public short IdMarca { get; set; }

    public short IdModVe { get; set; }

    public short IdEspeci { get; set; }

    public short? TipoCom { get; set; }

    public short? Pallets { get; set; }

    public float? RenEsp { get; set; }

    public float? CapCom { get; set; }

    public float? CapAce { get; set; }

    public string? TipoMot { get; set; }

    public string? PotMot { get; set; }

    public string? PulCub { get; set; }

    public short? NoCilin { get; set; }

    public float? Ancho { get; set; }

    public float? Largo { get; set; }

    public float? Altura { get; set; }

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

    public float? McubCar { get; set; }

    public float? TonCar { get; set; }

    public string? Obs { get; set; }
}
