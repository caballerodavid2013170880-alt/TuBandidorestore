using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Membresium
{
    public int Idmembreria { get; set; }

    public DateTime? Desde { get; set; }

    public DateTime? Hasta { get; set; }

    public int? UsuarioIdusuario { get; set; }

    public int? TransaccionIdtransaccion { get; set; }

    public virtual Transaccion? TransaccionIdtransaccionNavigation { get; set; }

    public virtual Usuario? UsuarioIdusuarioNavigation { get; set; }
}
