using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Logtransaccionesentidade
{
    public long Id { get; set; }

    public string? Nombreentidad { get; set; }

    public string? Identidad { get; set; }

    public string? Accion { get; set; }

    public string? Usuario { get; set; }

    public string? Fecha { get; set; }

    public string? Valoranterior { get; set; }

    public string? Valornuevo { get; set; }
}
