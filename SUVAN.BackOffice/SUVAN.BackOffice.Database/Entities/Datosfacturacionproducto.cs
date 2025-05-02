using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Datosfacturacionproducto
{
    public int Iddatosfacturacionproducto { get; set; }

    public int? Cantidad { get; set; }

    public string? Claveunidad { get; set; }

    public string? Claveprodserv { get; set; }

    public string? Descripcion { get; set; }

    public string? Noidentificacion { get; set; }

    public string? Objetoimp { get; set; }

    public string? Tipocomprobanteclave { get; set; }

    public string? Sucursal { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public decimal? Iva { get; set; }
}
