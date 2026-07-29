using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

/// <summary>
/// Tabla de asignación jerárquica unificada para todos los usuarios del sistema SUVAN
/// </summary>
public partial class UsuarioJerarquium
{
    /// <summary>
    /// Identificador único del registro de jerarquía de usuario
    /// </summary>
    public int IdUsuarioJerarquia { get; set; }

    /// <summary>
    /// Tipo de usuario: Admin, Usuario, Conductor, Mecanico
    /// </summary>
    public string TipoUsuario { get; set; } = null!;

    /// <summary>
    /// Identificador del usuario en su tabla correspondiente (Idadmin, Idusuario, Idconductor, IdMecanico)
    /// </summary>
    public int IdUsuario { get; set; }

    /// <summary>
    /// Identificador de la empresa asignada
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// Identificador de la región asignada (opcional)
    /// </summary>
    public int? IdRegion { get; set; }

    /// <summary>
    /// Identificador de la planta asignada (opcional)
    /// </summary>
    public int? IdPlanta { get; set; }

    /// <summary>
    /// Identificador de la zona asignada (opcional)
    /// </summary>
    public int? IdZona { get; set; }

    /// <summary>
    /// Identificador del depósito asignado (opcional)
    /// </summary>
    public int? IdDeposito { get; set; }

    /// <summary>
    /// Identificador del departamento asignado (opcional)
    /// </summary>
    public int? IdDepto { get; set; }

    /// <summary>
    /// Indica si es la jerarquía activa por defecto para el usuario (1=Sí, 0=No)
    /// </summary>
    public ulong EsPrincipal { get; set; }

    /// <summary>
    /// Estatus del registro (1=Activo, 0=Inactivo)
    /// </summary>
    public ulong Activo { get; set; }

    /// <summary>
    /// Fecha de registro de la jerarquía
    /// </summary>
    public DateTime FechaRegistro { get; set; }

    public virtual Deposito? IdDepositoNavigation { get; set; }

    public virtual Depto? IdDeptoNavigation { get; set; }

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Plantum? IdPlantaNavigation { get; set; }

    public virtual Region? IdRegionNavigation { get; set; }

    public virtual Zona? IdZonaNavigation { get; set; }
}
