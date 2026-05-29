using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Pago
{
    public int Idpago { get; set; }

    public decimal? Cantidad { get; set; }

    public int MetodopagoIdmetodopago { get; set; }

    public DateTime? Fecha { get; set; }

    public int UsuarioIdusuario { get; set; }

    public virtual Metodopago MetodopagoIdmetodopagoNavigation { get; set; } = null!;

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;
}
