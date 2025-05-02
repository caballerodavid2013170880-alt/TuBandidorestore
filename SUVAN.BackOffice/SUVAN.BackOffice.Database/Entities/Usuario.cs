using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Usuario
{
    public int Idusuario { get; set; }

    public string? Email { get; set; }

    public string? Hashpass { get; set; }

    public int? CodigopaisIdcodigopais { get; set; }

    public string? Telefono { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public string? CodigoAuth { get; set; }

    public ulong? Validado { get; set; }

    public ulong? Activo { get; set; }

    public string? Nombre { get; set; }

    public DateTime? CodigoExp { get; set; }

    public string? FirebaseId { get; set; }

    public virtual ICollection<CalificacionConductor> CalificacionConductors { get; set; } = new List<CalificacionConductor>();

    public virtual ICollection<CalificacionUsuario> CalificacionUsuarios { get; set; } = new List<CalificacionUsuario>();

    public virtual Codigopai? CodigopaisIdcodigopaisNavigation { get; set; }

    public virtual ICollection<Datosfacturacionreceptor> Datosfacturacionreceptors { get; set; } = new List<Datosfacturacionreceptor>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Favoritopersonal> Favoritopersonals { get; set; } = new List<Favoritopersonal>();

    public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();

    public virtual Fotografium? Fotografium { get; set; }

    public virtual ICollection<Membresium> Membresia { get; set; } = new List<Membresium>();

    public virtual Monedero? Monedero { get; set; }

    public virtual ICollection<Tokenpago> Tokenpagos { get; set; } = new List<Tokenpago>();

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();

    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();

    public virtual ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
}
