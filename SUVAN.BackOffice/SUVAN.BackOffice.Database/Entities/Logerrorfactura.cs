using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Logerrorfactura
{
    public int Idlogerrorfactura { get; set; }

    public int UsuarioIdusuario { get; set; }

    public string? Respuestaservicio { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public string? Descripcionerror { get; set; }

    public string? Xmlrequest { get; set; }
}
