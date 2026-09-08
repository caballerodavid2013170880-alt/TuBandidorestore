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

    public virtual ICollection<CombCarga> CombCargas { get; set; } = new List<CombCarga>();

    public virtual ICollection<Conductor> Conductors { get; set; } = new List<Conductor>();

    public virtual ICollection<Corridum> Corrida { get; set; } = new List<Corridum>();

    public virtual ICollection<Datosfacturacionemisor> Datosfacturacionemisors { get; set; } = new List<Datosfacturacionemisor>();

    public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();

    public virtual ICollection<Depto> Deptos { get; set; } = new List<Depto>();

    public virtual Regimenfiscalreceptor? IdregimenfiscalNavigation { get; set; }

    public virtual ICollection<Plantum> Planta { get; set; } = new List<Plantum>();

    public virtual ICollection<Politicascompensacion> Politicascompensacions { get; set; } = new List<Politicascompensacion>();

    public virtual ICollection<Preventivo> Preventivos { get; set; } = new List<Preventivo>();

    public virtual ICollection<PromocionEmpresa> PromocionEmpresas { get; set; } = new List<PromocionEmpresa>();

    public virtual ICollection<Promocion> Promocions { get; set; } = new List<Promocion>();

    public virtual ICollection<Region> Regions { get; set; } = new List<Region>();

    public virtual ICollection<RendMe> RendMes { get; set; } = new List<RendMe>();

    public virtual ICollection<Rutum> Ruta { get; set; } = new List<Rutum>();

    public virtual ICollection<TarCom> TarComs { get; set; } = new List<TarCom>();

    public virtual ICollection<TarIave> TarIaves { get; set; } = new List<TarIave>();

    public virtual ICollection<UsuarioJerarquium> UsuarioJerarquia { get; set; } = new List<UsuarioJerarquium>();

    public virtual ICollection<Variableempresa> Variableempresas { get; set; } = new List<Variableempresa>();

    public virtual ICollection<VehiculoDetalle> VehiculoDetalles { get; set; } = new List<VehiculoDetalle>();

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();

    public virtual ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();

    public virtual ICollection<Zona> Zonas { get; set; } = new List<Zona>();
}
