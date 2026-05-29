using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Favoritopersonal
{
    public int Idfavoritopersonal { get; set; }

    public int UsuarioIdusuario { get; set; }

    public string? Nombre { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public string? Direccion { get; set; }

    public ulong? Activo { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;
}
