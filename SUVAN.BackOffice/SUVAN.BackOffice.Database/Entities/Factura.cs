using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Factura
{
    public int Idfactura { get; set; }

    public int UsuarioIdusuario { get; set; }

    public int TransaccionIdtransaccion { get; set; }

    public string? Transaccionidpeticionwebservice { get; set; }

    public int? Folio { get; set; }

    public string? Serie { get; set; }

    public DateTime? Fechacreacion { get; set; }

    public DateTime? Fechaemision { get; set; }

    public string? Cadenaoriginal { get; set; }

    public string? Codigobarras { get; set; }

    public string? Nocertificadocsd { get; set; }

    public string? FoliofiscalUuid { get; set; }

    public DateTime? Fechatimbrado { get; set; }

    public string? Certificadosat { get; set; }

    public string? Sellosat { get; set; }

    public string? Xmltimbrado { get; set; }

    public string? Xmltimbradopac { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public virtual Transaccion TransaccionIdtransaccionNavigation { get; set; } = null!;

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;
}
