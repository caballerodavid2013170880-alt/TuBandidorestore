using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Transaccion
{
    public int Idtransaccion { get; set; }

    public int MetodopagoIdmetodopago { get; set; }

    public string? Terminacion { get; set; }

    public decimal? Cantidad { get; set; }

    public int TipotransaccionIdtipotransaccion { get; set; }

    public int UsuarioIdusuario { get; set; }

    public string? Numeroordenpay { get; set; }

    public string? Numeropeticionpay { get; set; }

    public int EstatustransaccionIdestatustransaccion { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public string? Codigo { get; set; }

    public DateTime? Codigoexp { get; set; }

    public virtual Estatustransaccion EstatustransaccionIdestatustransaccionNavigation { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Membresium> Membresia { get; set; } = new List<Membresium>();

    public virtual Metodopago MetodopagoIdmetodopagoNavigation { get; set; } = null!;

    public virtual Tipotransaccion TipotransaccionIdtipotransaccionNavigation { get; set; } = null!;

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;

    public virtual ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
}
