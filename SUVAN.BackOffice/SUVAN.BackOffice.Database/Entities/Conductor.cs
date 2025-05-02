using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Conductor
{
    public int Idconductor { get; set; }

    public string? Nombre { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public string? Rfc { get; set; }

    public string? Imagen { get; set; }

    public int? EmpresaIdempresa { get; set; }

    public string? Email { get; set; }

    public string? Hashpass { get; set; }

    public int? CodigopaisIdcodigopais { get; set; }

    public string? Telefono { get; set; }

    public string? CodigoAuth { get; set; }

    public DateTime? CodigoExp { get; set; }

    public ulong? Validado { get; set; }

    public string? Direccion { get; set; }

    public string? Curp { get; set; }

    public string? Ine { get; set; }

    public string? Tiposangre { get; set; }

    public string? Numerolicencia { get; set; }

    public string? Tipolicencia { get; set; }

    public string? Cif { get; set; }

    public string? Nombrecontacto { get; set; }

    public string? Telefonocontacto { get; set; }

    public int? Idregimenfiscal { get; set; }

    public decimal? Comisionfija { get; set; }

    public decimal? ComisionvariableKm { get; set; }

    public decimal? ComisionvariableIngresos { get; set; }

    public string? FirebaseId { get; set; }

    public virtual ICollection<CalificacionConductor> CalificacionConductors { get; set; } = new List<CalificacionConductor>();

    public virtual ICollection<CalificacionUsuario> CalificacionUsuarios { get; set; } = new List<CalificacionUsuario>();

    public virtual Codigopai? CodigopaisIdcodigopaisNavigation { get; set; }

    public virtual ICollection<CorridaAsignacion> CorridaAsignacions { get; set; } = new List<CorridaAsignacion>();

    public virtual Empresa? EmpresaIdempresaNavigation { get; set; }

    public virtual FotoConductor? FotoConductor { get; set; }

    public virtual Regimenfiscalreceptor? IdregimenfiscalNavigation { get; set; }

    public virtual ICollection<LiquidacionCabecera> LiquidacionCabeceras { get; set; } = new List<LiquidacionCabecera>();
}
