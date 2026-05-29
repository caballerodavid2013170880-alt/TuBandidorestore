using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Viaje
{
    public int Idviaje { get; set; }

    public int UsuarioIdusuario { get; set; }

    public sbyte EstatusviajeIdestatusviaje { get; set; }

    public DateTime? Fechaviaje { get; set; }

    public DateTime? Fechacompra { get; set; }

    public int ParadaInicio { get; set; }

    public int ParadaFin { get; set; }

    public int? TransaccionIdtransaccion { get; set; }

    public int? Numeropasajeros { get; set; }

    public DateTime? Vigenciareserva { get; set; }

    public int? CorridaAsignacionIdcorridaAsignacion { get; set; }

    public string? Boleto { get; set; }

    public DateTime? Fechacheckin { get; set; }

    public int? EmpresaIdempresa { get; set; }

    public DateTime? Fechacheckout { get; set; }

    public int? PromocionIdpromocion { get; set; }

    public int? DescuentoIddescuento { get; set; }

    public decimal? CostoTarifaUnitario { get; set; }

    public decimal? CostoFinalUnitario { get; set; }

    public decimal? CostoTarifaTotal { get; set; }

    public decimal? CostoFinalTotal { get; set; }

    public bool? Facturado { get; set; }

    public int? ViajeredondoIdviajeredondo { get; set; }

    public virtual CalificacionConductor? CalificacionConductor { get; set; }

    public virtual CalificacionUsuario? CalificacionUsuario { get; set; }

    public virtual CorridaAsignacion? CorridaAsignacionIdcorridaAsignacionNavigation { get; set; }

    public virtual Empresa? EmpresaIdempresaNavigation { get; set; }

    public virtual Estatusviaje EstatusviajeIdestatusviajeNavigation { get; set; } = null!;

    public virtual ICollection<Logcancelacionviaje> Logcancelacionviajes { get; set; } = new List<Logcancelacionviaje>();

    public virtual Paradum ParadaFinNavigation { get; set; } = null!;

    public virtual Paradum ParadaInicioNavigation { get; set; } = null!;

    public virtual Promocion? PromocionIdpromocionNavigation { get; set; }

    public virtual Transaccion? TransaccionIdtransaccionNavigation { get; set; }

    public virtual Usuario UsuarioIdusuarioNavigation { get; set; } = null!;

    public virtual Viajeredondo? ViajeredondoIdviajeredondoNavigation { get; set; }
}
