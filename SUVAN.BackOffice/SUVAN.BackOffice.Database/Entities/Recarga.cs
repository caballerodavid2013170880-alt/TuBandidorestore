using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Recarga
{
    public int Idrecarga { get; set; }

    public int MonederoUsuarioIdusuario { get; set; }

    public DateTime? Fecha { get; set; }

    public int MetodopagoIdmetodopago { get; set; }

    public virtual Metodopago MetodopagoIdmetodopagoNavigation { get; set; } = null!;

    public virtual Monedero MonederoUsuarioIdusuarioNavigation { get; set; } = null!;
}
