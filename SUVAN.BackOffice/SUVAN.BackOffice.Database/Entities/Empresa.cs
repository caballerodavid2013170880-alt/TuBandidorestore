using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Empresa
{
    public int Idempresa { get; set; }

    public string? Nombre { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public ulong? Activo { get; set; }

    public string? Rfc { get; set; }

    public string? Nombrecorto { get; set; }

    public int? Idregimenfiscal { get; set; }

    public virtual ICollection<AdminEmpresa> AdminEmpresas { get; set; } = new List<AdminEmpresa>();

    public virtual ICollection<Conductor> Conductors { get; set; } = new List<Conductor>();

    public virtual ICollection<Corridum> Corrida { get; set; } = new List<Corridum>();

    public virtual ICollection<Datosfacturacionemisor> Datosfacturacionemisors { get; set; } = new List<Datosfacturacionemisor>();

    public virtual Regimenfiscalreceptor? IdregimenfiscalNavigation { get; set; }

    public virtual ICollection<Politicascompensacion> Politicascompensacions { get; set; } = new List<Politicascompensacion>();

    public virtual ICollection<PromocionEmpresa> PromocionEmpresas { get; set; } = new List<PromocionEmpresa>();

    public virtual ICollection<Promocion> Promocions { get; set; } = new List<Promocion>();

    public virtual ICollection<Rutum> Ruta { get; set; } = new List<Rutum>();

    public virtual ICollection<Variableempresa> Variableempresas { get; set; } = new List<Variableempresa>();

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();

    public virtual ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
}
