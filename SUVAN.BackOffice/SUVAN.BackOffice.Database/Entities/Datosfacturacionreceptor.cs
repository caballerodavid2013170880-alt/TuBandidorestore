using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Datosfacturacionreceptor
{
    public int Iddatosfacturacionreceptor { get; set; }

    public int UsuarioIdusuario { get; set; }

    public string Rfc { get; set; } = null!;

    public string Nombreorazonsocial { get; set; } = null!;

    public int UsoscfdireceptorIdusoscfdireceptor { get; set; }

    public int RegimenfiscalreceptorIdregimenfiscalreceptor { get; set; }

    public string? Codigopostal { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual Regimenfiscalreceptor RegimenfiscalreceptorIdregimenfiscalreceptorNavigation { get; set; } = null!;

    public virtual Usoscfdireceptor UsoscfdireceptorIdusoscfdireceptorNavigation { get; set; } = null!;

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;
}
