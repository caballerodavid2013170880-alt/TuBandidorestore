using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static SUVAN.BackOffice.Models.StoredsProcedures.ModelsStoredsProcedures;

namespace SUVAN.BackOffice.Database.Entities;

public partial class SuvanDbContext : DbContext
{
    private readonly IConfiguration configuration;

    public SuvanDbContext()
    {
    }

    public SuvanDbContext(DbContextOptions<SuvanDbContext> options, IConfiguration configuration)
    : base(options)
    {
        this.configuration = configuration;
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminEmpresa> AdminEmpresas { get; set; }

    public virtual DbSet<Bitacoraloginweb> Bitacoraloginwebs { get; set; }

    public virtual DbSet<CalificacionConductor> CalificacionConductors { get; set; }

    public virtual DbSet<CalificacionCorridaasignacion> CalificacionCorridaasignacions { get; set; }

    public virtual DbSet<CalificacionUsuario> CalificacionUsuarios { get; set; }

    public virtual DbSet<CausaMantenimiento> CausaMantenimientos { get; set; }

    public virtual DbSet<Codigocorreo> Codigocorreos { get; set; }

    public virtual DbSet<Codigodescuento> Codigodescuentos { get; set; }

    public virtual DbSet<Codigopai> Codigopais { get; set; }

    public virtual DbSet<Conductor> Conductors { get; set; }

    public virtual DbSet<Contenido> Contenidos { get; set; }

    public virtual DbSet<Conversacionconexion> Conversacionconexions { get; set; }

    public virtual DbSet<Conversacionmensaje> Conversacionmensajes { get; set; }

    public virtual DbSet<Conversacionportal> Conversacionportals { get; set; }

    public virtual DbSet<Conversacionusuario> Conversacionusuarios { get; set; }

    public virtual DbSet<CorridaAsignacion> CorridaAsignacions { get; set; }

    public virtual DbSet<CorridaAsignacionParadum> CorridaAsignacionParada { get; set; }

    public virtual DbSet<CorridaDia> CorridaDias { get; set; }

    public virtual DbSet<CorridaLiquidacion> CorridaLiquidacions { get; set; }

    public virtual DbSet<CorridaParadum> CorridaParada { get; set; }

    public virtual DbSet<Corridum> Corrida { get; set; }

    public virtual DbSet<Datosfacturacionemisor> Datosfacturacionemisors { get; set; }

    public virtual DbSet<Datosfacturacionproducto> Datosfacturacionproductos { get; set; }

    public virtual DbSet<Datosfacturacionreceptor> Datosfacturacionreceptors { get; set; }

    public virtual DbSet<Depositosdisponible> Depositosdisponibles { get; set; }

    public virtual DbSet<Dia> Dias { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<EmpresaRutum> EmpresaRuta { get; set; }

    public virtual DbSet<Estatusestacion> Estatusestacions { get; set; }

    public virtual DbSet<Estatustransaccion> Estatustransaccions { get; set; }

    public virtual DbSet<Estatusviaje> Estatusviajes { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<FallaAuxilioVial> FallaAuxilioVials { get; set; }

    public virtual DbSet<Favorito> Favoritos { get; set; }

    public virtual DbSet<Favoritopersonal> Favoritopersonals { get; set; }

    public virtual DbSet<FotoConductor> FotoConductors { get; set; }

    public virtual DbSet<Fotografium> Fotografia { get; set; }

    public virtual DbSet<LiquidacionCabecera> LiquidacionCabeceras { get; set; }

    public virtual DbSet<LiquidacionDetalle> LiquidacionDetalles { get; set; }

    public virtual DbSet<Logcancelacionviaje> Logcancelacionviajes { get; set; }

    public virtual DbSet<Logerrorfactura> Logerrorfacturas { get; set; }

    public virtual DbSet<Logtransaccion> Logtransaccions { get; set; }

    public virtual DbSet<Logtransaccionesentidade> Logtransaccionesentidades { get; set; }

    public virtual DbSet<Mantenimiento> Mantenimientos { get; set; }

    public virtual DbSet<MantenimientoDetalle> MantenimientoDetalles { get; set; }

    public virtual DbSet<Mecanico> Mecanicos { get; set; }

    public virtual DbSet<Membresium> Membresia { get; set; }

    public virtual DbSet<Mensaje> Mensajes { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Metodopago> Metodopagos { get; set; }

    public virtual DbSet<Mfaportal> Mfaportals { get; set; }

    public virtual DbSet<Monedero> Monederos { get; set; }

    public virtual DbSet<MotivoAuxilioVial> MotivoAuxilioVials { get; set; }

    public virtual DbSet<NotificacionesSiniestro> NotificacionesSiniestros { get; set; }

    public virtual DbSet<Paradum> Parada { get; set; }

    public virtual DbSet<Perfil> Perfils { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Personalidadjuridica> Personalidadjuridicas { get; set; }

    public virtual DbSet<Politicascompensacion> Politicascompensacions { get; set; }

    public virtual DbSet<Preventivo> Preventivos { get; set; }

    public virtual DbSet<Promocion> Promocions { get; set; }

    public virtual DbSet<PromocionCorridum> PromocionCorrida { get; set; }

    public virtual DbSet<PromocionEmpresa> PromocionEmpresas { get; set; }

    public virtual DbSet<PromocionHorario> PromocionHorarios { get; set; }

    public virtual DbSet<PromocionRutum> PromocionRuta { get; set; }

    public virtual DbSet<Puntovirtual> Puntovirtuals { get; set; }

    public virtual DbSet<Recarga> Recargas { get; set; }

    public virtual DbSet<Regimenfiscalreceptor> Regimenfiscalreceptors { get; set; }

    public virtual DbSet<RutaParadum> RutaParada { get; set; }

    public virtual DbSet<Rutum> Ruta { get; set; }

    public virtual DbSet<Siniestro> Siniestros { get; set; }

    public virtual DbSet<Taller> Tallers { get; set; }

    public virtual DbSet<TarifaEscalonadum> TarifaEscalonada { get; set; }

    public virtual DbSet<TarifaGeneral> TarifaGenerals { get; set; }

    public virtual DbSet<TipoServicio> TipoServicios { get; set; }

    public virtual DbSet<Tipocodigodescuento> Tipocodigodescuentos { get; set; }

    public virtual DbSet<Tipocontenido> Tipocontenidos { get; set; }

    public virtual DbSet<Tipodescuento> Tipodescuentos { get; set; }

    public virtual DbSet<Tipoestacion> Tipoestacions { get; set; }

    public virtual DbSet<Tipofavorito> Tipofavoritos { get; set; }

    public virtual DbSet<Tipopromocion> Tipopromocions { get; set; }

    public virtual DbSet<Tipotarifa> Tipotarifas { get; set; }

    public virtual DbSet<Tipotransaccion> Tipotransaccions { get; set; }

    public virtual DbSet<Tipovariable> Tipovariables { get; set; }

    public virtual DbSet<Tipovehiculo> Tipovehiculos { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<Tokenpago> Tokenpagos { get; set; }

    public virtual DbSet<Transaccion> Transaccions { get; set; }

    public virtual DbSet<Transaccionordenstripe> Transaccionordenstripes { get; set; }

    public virtual DbSet<Usoscfdireceptor> Usoscfdireceptors { get; set; }

    public virtual DbSet<UsoscfdireceptorRegimenfiscalreceptor> UsoscfdireceptorRegimenfiscalreceptors { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Variable> Variables { get; set; }

    public virtual DbSet<Variableempresa> Variableempresas { get; set; }

    public virtual DbSet<Variableglobal> Variableglobals { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    public virtual DbSet<Vehiculoservicio> Vehiculoservicios { get; set; }

    public virtual DbSet<Viaje> Viajes { get; set; }

    public virtual DbSet<Viajeredondo> Viajeredondos { get; set; }

    public virtual DbSet<Zona> Zonas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.31-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        //NOTA: Se agrega este modelo para ejecutar sp
        modelBuilder.Entity<ModelstpBuscaServicio>().HasNoKey();
        modelBuilder.Entity<ModelstpRevisaEstacionalaRedonda>().HasNoKey();
        modelBuilder.Entity<ModelRutaConfiguracion>().HasNoKey();

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Idadmin).HasName("PRIMARY");

            entity.ToTable("admin");

            entity.HasIndex(e => e.PerfilIdperfil, "fk_admin_perfil1_idx");

            entity.Property(e => e.Idadmin).HasColumnName("idadmin");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .HasColumnName("email");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.FirebaseId)
                .HasMaxLength(60)
                .HasColumnName("firebase_id");
            entity.Property(e => e.Hashpassword)
                .HasMaxLength(45)
                .HasColumnName("hashpassword");
            entity.Property(e => e.Imagen)
                .HasMaxLength(500)
                .HasColumnName("imagen");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
            entity.Property(e => e.PerfilIdperfil).HasColumnName("perfil_idperfil");

            entity.HasOne(d => d.PerfilIdperfilNavigation).WithMany(p => p.Admins)
                .HasForeignKey(d => d.PerfilIdperfil)
                .HasConstraintName("fk_admin_perfil1");
        });

        modelBuilder.Entity<AdminEmpresa>(entity =>
        {
            entity.HasKey(e => new { e.AdminIdadmin, e.EmpresaIdempresa })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("admin_empresa");

            entity.HasIndex(e => e.AdminIdadmin, "fk_admin_empresa_admin1_idx");

            entity.HasIndex(e => e.EmpresaIdempresa, "fk_admin_empresa_empresa1_idx");

            entity.HasIndex(e => e.PerfilIdperfil, "fk_admin_empresa_perfil1_idx");

            entity.Property(e => e.AdminIdadmin).HasColumnName("admin_idadmin");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.PerfilIdperfil).HasColumnName("perfil_idperfil");
            entity.Property(e => e.Principal)
                .HasColumnType("bit(1)")
                .HasColumnName("principal");

            entity.HasOne(d => d.AdminIdadminNavigation).WithMany(p => p.AdminEmpresas)
                .HasForeignKey(d => d.AdminIdadmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_admin_empresa_admin1");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.AdminEmpresas)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_admin_empresa_empresa1");

            entity.HasOne(d => d.PerfilIdperfilNavigation).WithMany(p => p.AdminEmpresas)
                .HasForeignKey(d => d.PerfilIdperfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_admin_empresa_perfil1");
        });

        modelBuilder.Entity<Bitacoraloginweb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bitacoraloginweb");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Codigo)
                .HasMaxLength(100)
                .HasColumnName("codigo");
            entity.Property(e => e.Detalle)
                .HasMaxLength(250)
                .HasColumnName("detalle");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Error).HasColumnName("error");
            entity.Property(e => e.Fechaaccion)
                .HasColumnType("datetime")
                .HasColumnName("fechaaccion");
            entity.Property(e => e.Fechaexpiracodigo)
                .HasColumnType("datetime")
                .HasColumnName("fechaexpiracodigo");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
        });

        modelBuilder.Entity<CalificacionConductor>(entity =>
        {
            entity.HasKey(e => e.ViajeIdviaje).HasName("PRIMARY");

            entity.ToTable("calificacion_conductor");

            entity.HasIndex(e => e.ConductorIdconductor, "fk_conductor_calificacion_idx");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_usuario_calificacion_idx");

            entity.Property(e => e.ViajeIdviaje)
                .ValueGeneratedNever()
                .HasColumnName("viaje_idviaje");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.ConductorIdconductor).HasColumnName("conductor_idconductor");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(5000)
                .HasColumnName("mensaje");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");

            entity.HasOne(d => d.ConductorIdconductorNavigation).WithMany(p => p.CalificacionConductors)
                .HasForeignKey(d => d.ConductorIdconductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_conductor_calificacion");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.CalificacionConductors)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario_calificacion");

            entity.HasOne(d => d.ViajeIdviajeNavigation).WithOne(p => p.CalificacionConductor)
                .HasForeignKey<CalificacionConductor>(d => d.ViajeIdviaje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_viaje_calificacion");
        });

        modelBuilder.Entity<CalificacionCorridaasignacion>(entity =>
        {
            entity.HasKey(e => e.CorridaasignacionIdcorridaasignacion).HasName("PRIMARY");

            entity.ToTable("calificacion_corridaasignacion");

            entity.Property(e => e.CorridaasignacionIdcorridaasignacion)
                .ValueGeneratedNever()
                .HasColumnName("corridaasignacion_idcorridaasignacion");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
        });

        modelBuilder.Entity<CalificacionUsuario>(entity =>
        {
            entity.HasKey(e => e.ViajeIdviaje).HasName("PRIMARY");

            entity.ToTable("calificacion_usuario");

            entity.HasIndex(e => e.ConductorIdconductor, "fk_conductor_calificacion_idx2");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_usuario_calificacion_idx2");

            entity.Property(e => e.ViajeIdviaje)
                .ValueGeneratedNever()
                .HasColumnName("viaje_idviaje");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.ConductorIdconductor).HasColumnName("conductor_idconductor");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");

            entity.HasOne(d => d.ConductorIdconductorNavigation).WithMany(p => p.CalificacionUsuarios)
                .HasForeignKey(d => d.ConductorIdconductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_conductor_calificacion2");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.CalificacionUsuarios)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario_calificacion2");

            entity.HasOne(d => d.ViajeIdviajeNavigation).WithOne(p => p.CalificacionUsuario)
                .HasForeignKey<CalificacionUsuario>(d => d.ViajeIdviaje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_viaje_calificacion2");
        });

        modelBuilder.Entity<CausaMantenimiento>(entity =>
        {
            entity.HasKey(e => e.IdCausamantenimiento).HasName("PRIMARY");

            entity.ToTable("causa_mantenimiento");

            entity.HasIndex(e => e.PreventivoIdpreventivo, "fk_causamantenimiento_preventivo");

            entity.Property(e => e.IdCausamantenimiento).HasColumnName("id_causamantenimiento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.PreventivoIdpreventivo).HasColumnName("preventivo_idpreventivo");

            entity.HasOne(d => d.PreventivoIdpreventivoNavigation).WithMany(p => p.CausaMantenimientos)
                .HasForeignKey(d => d.PreventivoIdpreventivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_causamantenimiento_preventivo");
        });

        modelBuilder.Entity<Codigocorreo>(entity =>
        {
            entity.HasKey(e => e.Idcodigocorreo).HasName("PRIMARY");

            entity.ToTable("codigocorreo");

            entity.HasIndex(e => e.CodigodescuentoIdcodigodescuento, "codigocorreo_FK");

            entity.Property(e => e.Idcodigocorreo).HasColumnName("idcodigocorreo");
            entity.Property(e => e.CodigodescuentoIdcodigodescuento).HasColumnName("codigodescuento_idcodigodescuento");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .HasColumnName("email");

            entity.HasOne(d => d.CodigodescuentoIdcodigodescuentoNavigation).WithMany(p => p.Codigocorreos)
                .HasForeignKey(d => d.CodigodescuentoIdcodigodescuento)
                .HasConstraintName("codigocorreo_FK");
        });

        modelBuilder.Entity<Codigodescuento>(entity =>
        {
            entity.HasKey(e => e.Idcodigodescuento).HasName("PRIMARY");

            entity.ToTable("codigodescuento");

            entity.HasIndex(e => e.TipocodigodescuentoIdtipocodigodescuento, "fk_codigodescuento_tipocodigodescuento1_idx");

            entity.HasIndex(e => e.TipodescuentoIdtipodescuento1, "fk_codigodescuento_tipodescuento2_idx");

            entity.Property(e => e.Idcodigodescuento).HasColumnName("idcodigodescuento");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.Codigo)
                .HasMaxLength(6)
                .HasColumnName("codigo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.TipocodigodescuentoIdtipocodigodescuento).HasColumnName("tipocodigodescuento_idtipocodigodescuento");
            entity.Property(e => e.TipodescuentoIdtipodescuento1).HasColumnName("tipodescuento_idtipodescuento1");
            entity.Property(e => e.Vigenciadesde)
                .HasColumnType("datetime")
                .HasColumnName("vigenciadesde");
            entity.Property(e => e.Vigenciahasta)
                .HasColumnType("datetime")
                .HasColumnName("vigenciahasta");

            entity.HasOne(d => d.TipocodigodescuentoIdtipocodigodescuentoNavigation).WithMany(p => p.Codigodescuentos)
                .HasForeignKey(d => d.TipocodigodescuentoIdtipocodigodescuento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_codigodescuento_tipocodigodescuento1");

            entity.HasOne(d => d.TipodescuentoIdtipodescuento1Navigation).WithMany(p => p.Codigodescuentos)
                .HasForeignKey(d => d.TipodescuentoIdtipodescuento1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_codigodescuento_tipodescuento2");
        });

        modelBuilder.Entity<Codigopai>(entity =>
        {
            entity.HasKey(e => e.Idcodigopais).HasName("PRIMARY");

            entity.ToTable("codigopais");

            entity.Property(e => e.Idcodigopais)
                .ValueGeneratedNever()
                .HasColumnName("idcodigopais");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Pais)
                .HasMaxLength(250)
                .HasColumnName("pais");
        });

        modelBuilder.Entity<Conductor>(entity =>
        {
            entity.HasKey(e => e.Idconductor).HasName("PRIMARY");

            entity.ToTable("conductor");

            entity.HasIndex(e => e.Idregimenfiscal, "conductor_regimenfiscalreceptor_FK");

            entity.HasIndex(e => e.CodigopaisIdcodigopais, "fk_conductor_codigopais1_idx");

            entity.HasIndex(e => e.EmpresaIdempresa, "fk_conductor_empresa1_idx");

            entity.Property(e => e.Idconductor).HasColumnName("idconductor");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Cif)
                .HasMaxLength(20)
                .HasColumnName("cif");
            entity.Property(e => e.CodigoAuth)
                .HasMaxLength(45)
                .HasColumnName("codigo_auth");
            entity.Property(e => e.CodigoExp)
                .HasColumnType("datetime")
                .HasColumnName("codigo_exp");
            entity.Property(e => e.CodigopaisIdcodigopais).HasColumnName("codigopais_idcodigopais");
            entity.Property(e => e.Comisionfija)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("comisionfija");
            entity.Property(e => e.ComisionvariableIngresos)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("comisionvariableIngresos");
            entity.Property(e => e.ComisionvariableKm)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("comisionvariableKM");
            entity.Property(e => e.Curp)
                .HasMaxLength(20)
                .HasColumnName("curp");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .HasColumnName("email");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.FirebaseId)
                .HasMaxLength(260)
                .HasColumnName("firebase_id");
            entity.Property(e => e.Hashpass)
                .HasMaxLength(150)
                .HasColumnName("hashpass");
            entity.Property(e => e.Idregimenfiscal).HasColumnName("idregimenfiscal");
            entity.Property(e => e.Imagen).HasColumnName("imagen");
            entity.Property(e => e.Ine)
                .HasMaxLength(50)
                .HasColumnName("ine");
            entity.Property(e => e.Nacionalidad)
                .HasMaxLength(250)
                .HasColumnName("nacionalidad");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
            entity.Property(e => e.Nombrecontacto)
                .HasMaxLength(250)
                .HasColumnName("nombrecontacto");
            entity.Property(e => e.Nss)
                .HasMaxLength(100)
                .HasColumnName("nss");
            entity.Property(e => e.Numerolicencia)
                .HasMaxLength(20)
                .HasColumnName("numerolicencia");
            entity.Property(e => e.Rfc)
                .HasMaxLength(25)
                .HasColumnName("rfc");
            entity.Property(e => e.Telefono)
                .HasMaxLength(45)
                .HasColumnName("telefono");
            entity.Property(e => e.Telefonocontacto)
                .HasMaxLength(45)
                .HasColumnName("telefonocontacto");
            entity.Property(e => e.TipoVisa)
                .HasMaxLength(250)
                .HasColumnName("tipoVisa");
            entity.Property(e => e.Tipolicencia)
                .HasMaxLength(100)
                .HasColumnName("tipolicencia");
            entity.Property(e => e.Tiposangre)
                .HasMaxLength(10)
                .HasColumnName("tiposangre");
            entity.Property(e => e.Validado)
                .HasColumnType("bit(1)")
                .HasColumnName("validado");
            entity.Property(e => e.Visa)
                .HasColumnType("bit(1)")
                .HasColumnName("visa");

            entity.HasOne(d => d.CodigopaisIdcodigopaisNavigation).WithMany(p => p.Conductors)
                .HasForeignKey(d => d.CodigopaisIdcodigopais)
                .HasConstraintName("fk_conductor_codigopais1");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.Conductors)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .HasConstraintName("fk_conductor_empresa1");

            entity.HasOne(d => d.IdregimenfiscalNavigation).WithMany(p => p.Conductors)
                .HasForeignKey(d => d.Idregimenfiscal)
                .HasConstraintName("conductor_regimenfiscalreceptor_FK");
        });

        modelBuilder.Entity<Contenido>(entity =>
        {
            entity.HasKey(e => e.Idcontenido).HasName("PRIMARY");

            entity.ToTable("contenido");

            entity.HasIndex(e => e.TipocontenidoIdtipocontenido, "fk_contenido_tipocontenido1_idx");

            entity.Property(e => e.Idcontenido).HasColumnName("idcontenido");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Html).HasColumnName("html");
            entity.Property(e => e.Imagen).HasColumnName("imagen");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.TipocontenidoIdtipocontenido).HasColumnName("tipocontenido_idtipocontenido");
            entity.Property(e => e.Titulo)
                .HasMaxLength(250)
                .HasColumnName("titulo");

            entity.HasOne(d => d.TipocontenidoIdtipocontenidoNavigation).WithMany(p => p.Contenidos)
                .HasForeignKey(d => d.TipocontenidoIdtipocontenido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contenido_tipocontenido1");
        });

        modelBuilder.Entity<Conversacionconexion>(entity =>
        {
            entity.HasKey(e => new { e.ConversacionConexionId, e.ConversacionId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("conversacionconexion");

            entity.HasIndex(e => e.ConversacionConexionId, "ConversacionConexionId_UNIQUE").IsUnique();

            entity.HasIndex(e => e.ConversacionId, "FK_Conversacion_Conexion_idx");

            entity.Property(e => e.ConversacionConexionId).ValueGeneratedOnAdd();
            entity.Property(e => e.ConexionId).HasMaxLength(100);
            entity.Property(e => e.FechaHoraCreacion).HasColumnType("datetime");
            entity.Property(e => e.TokenAcceso).HasMaxLength(200);

            entity.HasOne(d => d.Conversacion).WithMany(p => p.Conversacionconexions)
                .HasForeignKey(d => d.ConversacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conversacion_Conexion");
        });

        modelBuilder.Entity<Conversacionmensaje>(entity =>
        {
            entity.HasKey(e => new { e.ConversacionMensajeId, e.ConversacionId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("conversacionmensaje");

            entity.HasIndex(e => e.ConversacionMensajeId, "ConversacionMensajeId_UNIQUE").IsUnique();

            entity.HasIndex(e => e.ConversacionId, "FK_ConversacionMensaje_Conversacion_idx");

            entity.HasIndex(e => e.MensajeId, "FK_ConversacionMensaje_Mensaje_idx");

            entity.Property(e => e.ConversacionMensajeId).ValueGeneratedOnAdd();
            entity.Property(e => e.Error)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Estatus).HasComment("0=No Enviado\n1=Enviado\n-1=Error");
            entity.Property(e => e.FechaEnvio).HasColumnType("datetime");
            entity.Property(e => e.MensajeError).HasMaxLength(200);

            entity.HasOne(d => d.Conversacion).WithMany(p => p.Conversacionmensajes)
                .HasForeignKey(d => d.ConversacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConversacionMensaje_Conversacion");

            entity.HasOne(d => d.Mensaje).WithMany(p => p.Conversacionmensajes)
                .HasForeignKey(d => d.MensajeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConversacionMensaje_Mensaje");
        });

        modelBuilder.Entity<Conversacionportal>(entity =>
        {
            entity.HasKey(e => e.ConversacionId).HasName("PRIMARY");

            entity.ToTable("conversacionportal");

            entity.HasIndex(e => e.ConversacionId, "ConversacionId_UNIQUE").IsUnique();

            entity.Property(e => e.EstatusConversacion)
                .HasDefaultValueSql("b'1'")
                .HasComment("1=Activo\n0=Cerrada")
                .HasColumnType("bit(1)");
            entity.Property(e => e.FechaHoraCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TipoConversacion).HasComment("0=Empresa\n1=Ruta\n2=Operador");
            entity.Property(e => e.Titulo).HasMaxLength(150);
        });

        modelBuilder.Entity<Conversacionusuario>(entity =>
        {
            entity.HasKey(e => new { e.ConversacionUsuarioId, e.ConversacionId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("conversacionusuario");

            entity.HasIndex(e => e.ConversacionUsuarioId, "ConversacionUsuarioId_UNIQUE").IsUnique();

            entity.HasIndex(e => e.ConversacionId, "FK_ConversacionUsuario_Conversacion_idx");

            entity.Property(e => e.ConversacionUsuarioId).ValueGeneratedOnAdd();
            entity.Property(e => e.TipoUsuario).HasMaxLength(45);

            entity.HasOne(d => d.Conversacion).WithMany(p => p.Conversacionusuarios)
                .HasForeignKey(d => d.ConversacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConversacionUsuario_Conversacion");
        });

        modelBuilder.Entity<CorridaAsignacion>(entity =>
        {
            entity.HasKey(e => e.IdcorridaAsignacion).HasName("PRIMARY");

            entity.ToTable("corrida_asignacion");

            entity.HasIndex(e => e.ConductorIdconductor, "fk_corrida_ejecucion_conductor1_idx");

            entity.HasIndex(e => e.CorridaIdcorrida, "fk_corrida_ejecucion_corrida1_idx");

            entity.HasIndex(e => e.VehiculoIdvehiculo, "fk_corrida_ejecucion_vehiculo1_idx");

            entity.HasIndex(e => e.EstatusviajeIdestatusviaje, "fk_corridaejecucion_estatusviaje_idx");

            entity.Property(e => e.IdcorridaAsignacion).HasColumnName("idcorrida_asignacion");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.ConductorIdconductor).HasColumnName("conductor_idconductor");
            entity.Property(e => e.CorridaIdcorrida).HasColumnName("corrida_idcorrida");
            entity.Property(e => e.CurrentLat)
                .HasPrecision(12, 8)
                .HasColumnName("current_lat");
            entity.Property(e => e.CurrentLong)
                .HasPrecision(12, 8)
                .HasColumnName("current_long");
            entity.Property(e => e.EstatusviajeIdestatusviaje)
                .HasDefaultValueSql("'0'")
                .HasColumnName("estatusviaje_idestatusviaje");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdestacionActual).HasColumnName("idestacion_actual");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(5000)
                .HasColumnName("mensaje");
            entity.Property(e => e.VehiculoIdvehiculo).HasColumnName("vehiculo_idvehiculo");

            entity.HasOne(d => d.ConductorIdconductorNavigation).WithMany(p => p.CorridaAsignacions)
                .HasForeignKey(d => d.ConductorIdconductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_corrida_ejecucion_conductor1");

            entity.HasOne(d => d.CorridaIdcorridaNavigation).WithMany(p => p.CorridaAsignacions)
                .HasForeignKey(d => d.CorridaIdcorrida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_corrida_ejecucion_corrida1");

            entity.HasOne(d => d.EstatusviajeIdestatusviajeNavigation).WithMany(p => p.CorridaAsignacions)
                .HasForeignKey(d => d.EstatusviajeIdestatusviaje)
                .HasConstraintName("fk_corridaejecucion_estatusviaje");

            entity.HasOne(d => d.VehiculoIdvehiculoNavigation).WithMany(p => p.CorridaAsignacions)
                .HasForeignKey(d => d.VehiculoIdvehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_corrida_ejecucion_vehiculo1");
        });

        modelBuilder.Entity<CorridaAsignacionParadum>(entity =>
        {
            entity.HasKey(e => new { e.CorridaAsignacionIdcorridaAsignacion, e.ParadaIdparada })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("corrida_asignacion_parada");

            entity.HasIndex(e => e.EstatusestacionIdestatusestacion, "FKEstatusEstacion_idx");

            entity.Property(e => e.CorridaAsignacionIdcorridaAsignacion).HasColumnName("corrida_asignacion_idcorrida_asignacion");
            entity.Property(e => e.ParadaIdparada).HasColumnName("parada_idparada");
            entity.Property(e => e.Bajan).HasColumnName("bajan");
            entity.Property(e => e.Bajaron).HasColumnName("bajaron");
            entity.Property(e => e.Espacios).HasColumnName("espacios");
            entity.Property(e => e.EstatusestacionIdestatusestacion).HasColumnName("estatusestacion_idestatusestacion");
            entity.Property(e => e.Suben).HasColumnName("suben");
            entity.Property(e => e.Subieron).HasColumnName("subieron");

            entity.HasOne(d => d.EstatusestacionIdestatusestacionNavigation).WithMany(p => p.CorridaAsignacionParada)
                .HasForeignKey(d => d.EstatusestacionIdestatusestacion)
                .HasConstraintName("FKEstatusEstacion");
        });

        modelBuilder.Entity<CorridaDia>(entity =>
        {
            entity.HasKey(e => new { e.DiasIddias, e.CorridaIdcorrida })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("corrida_dias");

            entity.HasIndex(e => e.CorridaIdcorrida, "fk_table1_corrida2_idx");

            entity.HasIndex(e => e.DiasIddias, "fk_table1_dias1_idx");

            entity.Property(e => e.DiasIddias)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("dias_iddias");
            entity.Property(e => e.CorridaIdcorrida).HasColumnName("corrida_idcorrida");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");

            entity.HasOne(d => d.CorridaIdcorridaNavigation).WithMany(p => p.CorridaDia)
                .HasForeignKey(d => d.CorridaIdcorrida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_table1_corrida2");

            entity.HasOne(d => d.DiasIddiasNavigation).WithMany(p => p.CorridaDia)
                .HasForeignKey(d => d.DiasIddias)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_table1_dias1");
        });

        modelBuilder.Entity<CorridaLiquidacion>(entity =>
        {
            entity.HasKey(e => e.IdCorridaAsignacion).HasName("PRIMARY");

            entity.ToTable("corrida_liquidacion");

            entity.HasIndex(e => e.IdCorridaAsignacion, "fk_corrida_asignacion_idx");

            entity.Property(e => e.IdCorridaAsignacion)
                .ValueGeneratedNever()
                .HasColumnName("id_corrida_asignacion");
            entity.Property(e => e.Liquidado).HasColumnName("liquidado");
            entity.Property(e => e.MontoComision)
                .HasPrecision(10, 2)
                .HasColumnName("monto_comision");
            entity.Property(e => e.MontoPagado)
                .HasPrecision(10, 2)
                .HasColumnName("monto_pagado");

            entity.HasOne(d => d.IdCorridaAsignacionNavigation).WithOne(p => p.CorridaLiquidacion)
                .HasForeignKey<CorridaLiquidacion>(d => d.IdCorridaAsignacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_corrida_asignacion");
        });

        modelBuilder.Entity<CorridaParadum>(entity =>
        {
            entity.HasKey(e => new { e.CorridaIdcorrida, e.ParadaIdparada })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("corrida_parada");

            entity.HasIndex(e => e.CorridaIdcorrida, "fk_table1_corrida1_idx");

            entity.HasIndex(e => e.ParadaIdparada, "fk_table1_parada1_idx");

            entity.Property(e => e.CorridaIdcorrida).HasColumnName("corrida_idcorrida");
            entity.Property(e => e.ParadaIdparada).HasColumnName("parada_idparada");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Hora)
                .HasColumnType("time")
                .HasColumnName("hora");

            entity.HasOne(d => d.CorridaIdcorridaNavigation).WithMany(p => p.CorridaParada)
                .HasForeignKey(d => d.CorridaIdcorrida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_table1_corrida1");

            entity.HasOne(d => d.ParadaIdparadaNavigation).WithMany(p => p.CorridaParada)
                .HasForeignKey(d => d.ParadaIdparada)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_table1_parada1");
        });

        modelBuilder.Entity<Corridum>(entity =>
        {
            entity.HasKey(e => e.Idcorrida).HasName("PRIMARY");

            entity.ToTable("corrida");

            entity.HasIndex(e => e.EmpresaIdempresa, "corrida_FK");

            entity.HasIndex(e => e.RutaIdruta, "fk_corrida_ruta1_idx");

            entity.Property(e => e.Idcorrida).HasColumnName("idcorrida");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.HoraFin)
                .HasColumnType("time")
                .HasColumnName("horaFin");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("time")
                .HasColumnName("horaInicio");
            entity.Property(e => e.RutaIdruta).HasColumnName("ruta_idruta");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.Corrida)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .HasConstraintName("corrida_FK");

            entity.HasOne(d => d.RutaIdrutaNavigation).WithMany(p => p.Corrida)
                .HasForeignKey(d => d.RutaIdruta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_corrida_ruta1");
        });

        modelBuilder.Entity<Datosfacturacionemisor>(entity =>
        {
            entity.HasKey(e => e.Iddatosfacturacionemisor).HasName("PRIMARY");

            entity.ToTable("datosfacturacionemisor");

            entity.HasIndex(e => e.EmpresaIdempresa, "fk_datosfacturacionemisor_usuario_idx");

            entity.HasIndex(e => e.Iddatosfacturacionemisor, "fk_iddatosfacturacionemisor_idx");

            entity.Property(e => e.Iddatosfacturacionemisor).HasColumnName("iddatosfacturacionemisor");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Folio).HasColumnName("folio");
            entity.Property(e => e.LugarexpedicionCp)
                .HasMaxLength(10)
                .HasColumnName("lugarexpedicionCP");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
            entity.Property(e => e.Regimenfiscal)
                .HasMaxLength(20)
                .HasColumnName("regimenfiscal");
            entity.Property(e => e.Serie)
                .HasMaxLength(50)
                .HasColumnName("serie");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.Datosfacturacionemisors)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_datosfacturacionemisor_usuario");
        });

        modelBuilder.Entity<Datosfacturacionproducto>(entity =>
        {
            entity.HasKey(e => e.Iddatosfacturacionproducto).HasName("PRIMARY");

            entity.ToTable("datosfacturacionproducto");

            entity.HasIndex(e => e.Iddatosfacturacionproducto, "fk_iddatosfacturacionproducto_idx");

            entity.Property(e => e.Iddatosfacturacionproducto).HasColumnName("iddatosfacturacionproducto");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Claveprodserv)
                .HasMaxLength(50)
                .HasColumnName("claveprodserv");
            entity.Property(e => e.Claveunidad)
                .HasMaxLength(50)
                .HasColumnName("claveunidad");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Iva)
                .HasPrecision(10, 2)
                .HasColumnName("iva");
            entity.Property(e => e.Noidentificacion)
                .HasMaxLength(50)
                .HasColumnName("noidentificacion");
            entity.Property(e => e.Objetoimp)
                .HasMaxLength(50)
                .HasColumnName("objetoimp");
            entity.Property(e => e.Sucursal)
                .HasMaxLength(250)
                .HasColumnName("sucursal");
            entity.Property(e => e.Tipocomprobanteclave)
                .HasMaxLength(250)
                .HasColumnName("tipocomprobanteclave");
        });

        modelBuilder.Entity<Datosfacturacionreceptor>(entity =>
        {
            entity.HasKey(e => e.Iddatosfacturacionreceptor).HasName("PRIMARY");

            entity.ToTable("datosfacturacionreceptor");

            entity.HasIndex(e => e.RegimenfiscalreceptorIdregimenfiscalreceptor, "fk_datosfacturacionreceptor_regimenfiscalcfdireceptor_idx");

            entity.HasIndex(e => e.UsoscfdireceptorIdusoscfdireceptor, "fk_datosfacturacionreceptor_usuario2_idx");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_datosfacturacionreceptor_usuario_idx");

            entity.HasIndex(e => e.Iddatosfacturacionreceptor, "fk_iddatosfacturacionreceptor_idx");

            entity.Property(e => e.Iddatosfacturacionreceptor).HasColumnName("iddatosfacturacionreceptor");
            entity.Property(e => e.Codigopostal)
                .HasMaxLength(5)
                .HasColumnName("codigopostal");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Nombreorazonsocial)
                .HasMaxLength(500)
                .HasColumnName("nombreorazonsocial");
            entity.Property(e => e.RegimenfiscalreceptorIdregimenfiscalreceptor).HasColumnName("regimenfiscalreceptor_idregimenfiscalreceptor");
            entity.Property(e => e.Rfc)
                .HasMaxLength(15)
                .HasColumnName("rfc");
            entity.Property(e => e.UsoscfdireceptorIdusoscfdireceptor).HasColumnName("usoscfdireceptor_idusoscfdireceptor");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");

            entity.HasOne(d => d.RegimenfiscalreceptorIdregimenfiscalreceptorNavigation).WithMany(p => p.Datosfacturacionreceptors)
                .HasForeignKey(d => d.RegimenfiscalreceptorIdregimenfiscalreceptor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_datosfacturacionreceptor_regimenfiscalcfdireceptor");

            entity.HasOne(d => d.UsoscfdireceptorIdusoscfdireceptorNavigation).WithMany(p => p.Datosfacturacionreceptors)
                .HasForeignKey(d => d.UsoscfdireceptorIdusoscfdireceptor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_datosfacturacionreceptor_usoscfdireceptor");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.Datosfacturacionreceptors)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_datosfacturacionreceptor_usuario");
        });

        modelBuilder.Entity<Depositosdisponible>(entity =>
        {
            entity.HasKey(e => e.IdDeposito).HasName("PRIMARY");

            entity.ToTable("depositosdisponibles");

            entity.HasIndex(e => e.TallerId, "fk_deposito_taller");

            entity.HasIndex(e => e.ZonaId, "fk_deposito_zona");

            entity.Property(e => e.IdDeposito).HasColumnName("id_deposito");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.DepositoNombre)
                .HasMaxLength(100)
                .HasColumnName("deposito_nombre");
            entity.Property(e => e.TallerId).HasColumnName("taller_id");
            entity.Property(e => e.ZonaId).HasColumnName("zona_id");

            entity.HasOne(d => d.Taller).WithMany(p => p.Depositosdisponibles)
                .HasForeignKey(d => d.TallerId)
                .HasConstraintName("fk_deposito_taller");

            entity.HasOne(d => d.Zona).WithMany(p => p.Depositosdisponibles)
                .HasForeignKey(d => d.ZonaId)
                .HasConstraintName("fk_deposito_zona");
        });

        modelBuilder.Entity<Dia>(entity =>
        {
            entity.HasKey(e => e.Iddias).HasName("PRIMARY");

            entity.ToTable("dias");

            entity.Property(e => e.Iddias)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("iddias");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
            entity.Property(e => e.Orden).HasColumnName("orden");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.Idempresa).HasName("PRIMARY");

            entity.ToTable("empresa");

            entity.HasIndex(e => e.Idregimenfiscal, "empresa_regimenfiscalreceptor_fk");

            entity.Property(e => e.Idempresa).HasColumnName("idempresa");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Idregimenfiscal).HasColumnName("idregimenfiscal");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
            entity.Property(e => e.Nombrecorto)
                .HasMaxLength(100)
                .HasColumnName("nombrecorto");
            entity.Property(e => e.Rfc)
                .HasMaxLength(25)
                .HasColumnName("rfc");

            entity.HasOne(d => d.IdregimenfiscalNavigation).WithMany(p => p.Empresas)
                .HasForeignKey(d => d.Idregimenfiscal)
                .HasConstraintName("empresa_regimenfiscalreceptor_fk");
        });

        modelBuilder.Entity<EmpresaRutum>(entity =>
        {
            entity.HasKey(e => new { e.EmpresaIdempresa, e.RutaIdruta })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("empresa_ruta");

            entity.HasIndex(e => e.EmpresaIdempresa, "fk_table1_empresa1_idx");

            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.RutaIdruta).HasColumnName("ruta_idruta");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Recharegistro)
                .HasColumnType("datetime")
                .HasColumnName("recharegistro");
            entity.Property(e => e.TipotarifaIdtipotarifa)
                .HasDefaultValueSql("'1'")
                .HasColumnName("tipotarifa_idtipotarifa");
        });

        modelBuilder.Entity<Estatusestacion>(entity =>
        {
            entity.HasKey(e => e.Idestatusestacion).HasName("PRIMARY");

            entity.ToTable("estatusestacion");

            entity.Property(e => e.Idestatusestacion)
                .ValueGeneratedNever()
                .HasColumnName("idestatusestacion");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Estatustransaccion>(entity =>
        {
            entity.HasKey(e => e.Idestatustransaccion).HasName("PRIMARY");

            entity.ToTable("estatustransaccion");

            entity.Property(e => e.Idestatustransaccion).HasColumnName("idestatustransaccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Estatusviaje>(entity =>
        {
            entity.HasKey(e => e.Idestatusviaje).HasName("PRIMARY");

            entity.ToTable("estatusviaje");

            entity.Property(e => e.Idestatusviaje)
                .ValueGeneratedNever()
                .HasColumnName("idestatusviaje");
            entity.Property(e => e.Estatusviajecol)
                .HasMaxLength(45)
                .HasColumnName("estatusviajecol");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Idfactura).HasName("PRIMARY");

            entity.ToTable("factura");

            entity.HasIndex(e => e.TransaccionIdtransaccion, "fk_factura_transaccion_idx");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_factura_usuario_idx");

            entity.HasIndex(e => e.Idfactura, "fk_idfactura_idx");

            entity.Property(e => e.Idfactura).HasColumnName("idfactura");
            entity.Property(e => e.Cadenaoriginal)
                .HasColumnType("mediumtext")
                .HasColumnName("cadenaoriginal");
            entity.Property(e => e.Certificadosat)
                .HasColumnType("mediumtext")
                .HasColumnName("certificadosat");
            entity.Property(e => e.Codigobarras)
                .HasColumnType("mediumtext")
                .HasColumnName("codigobarras");
            entity.Property(e => e.Fechacreacion)
                .HasColumnType("datetime")
                .HasColumnName("fechacreacion");
            entity.Property(e => e.Fechaemision)
                .HasColumnType("datetime")
                .HasColumnName("fechaemision");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Fechatimbrado)
                .HasColumnType("datetime")
                .HasColumnName("fechatimbrado");
            entity.Property(e => e.Folio).HasColumnName("folio");
            entity.Property(e => e.FoliofiscalUuid)
                .HasMaxLength(50)
                .HasColumnName("foliofiscalUUID");
            entity.Property(e => e.Nocertificadocsd)
                .HasColumnType("mediumtext")
                .HasColumnName("nocertificadocsd");
            entity.Property(e => e.Sellosat)
                .HasColumnType("mediumtext")
                .HasColumnName("sellosat");
            entity.Property(e => e.Serie)
                .HasMaxLength(50)
                .HasColumnName("serie");
            entity.Property(e => e.TransaccionIdtransaccion).HasColumnName("transaccion_idtransaccion");
            entity.Property(e => e.Transaccionidpeticionwebservice)
                .HasMaxLength(50)
                .HasColumnName("transaccionidpeticionwebservice");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");
            entity.Property(e => e.Xmltimbrado)
                .HasColumnType("mediumtext")
                .HasColumnName("xmltimbrado");
            entity.Property(e => e.Xmltimbradopac)
                .HasColumnType("mediumtext")
                .HasColumnName("xmltimbradopac");

            entity.HasOne(d => d.TransaccionIdtransaccionNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.TransaccionIdtransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_factura_transaccion");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_factura_usuario");
        });

        modelBuilder.Entity<FallaAuxilioVial>(entity =>
        {
            entity.HasKey(e => e.Idfalla).HasName("PRIMARY");

            entity.ToTable("falla_auxilio_vial");

            entity.Property(e => e.Idfalla).HasColumnName("idfalla");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Favorito>(entity =>
        {
            entity.HasKey(e => new { e.UsuarioIdusuario, e.TipofavoritoIdtipofavorito })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("favorito");

            entity.HasIndex(e => e.TipofavoritoIdtipofavorito, "fk_favorito_tipofavorito1_idx");

            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");
            entity.Property(e => e.TipofavoritoIdtipofavorito).HasColumnName("tipofavorito_idtipofavorito");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(2000)
                .HasColumnName("direccion");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Latitud)
                .HasPrecision(10, 8)
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasPrecision(11, 8)
                .HasColumnName("longitud");

            entity.HasOne(d => d.TipofavoritoIdtipofavoritoNavigation).WithMany(p => p.Favoritos)
                .HasForeignKey(d => d.TipofavoritoIdtipofavorito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_favorito_tipofavorito1");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.Favoritos)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_favorito_usuario1");
        });

        modelBuilder.Entity<Favoritopersonal>(entity =>
        {
            entity.HasKey(e => e.Idfavoritopersonal).HasName("PRIMARY");

            entity.ToTable("favoritopersonal");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_favoritopersonal_usuario1_idx");

            entity.Property(e => e.Idfavoritopersonal).HasColumnName("idfavoritopersonal");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(2000)
                .HasColumnName("direccion");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Latitud)
                .HasPrecision(10, 8)
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasPrecision(11, 8)
                .HasColumnName("longitud");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.Favoritopersonals)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_favoritopersonal_usuario1");
        });

        modelBuilder.Entity<FotoConductor>(entity =>
        {
            entity.HasKey(e => e.ConductorIdconductor).HasName("PRIMARY");

            entity.ToTable("foto_conductor");

            entity.HasIndex(e => e.ConductorIdconductor, "fk_table1_conductor1_idx");

            entity.Property(e => e.ConductorIdconductor)
                .ValueGeneratedNever()
                .HasColumnName("conductor_idconductor");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Imagen).HasColumnName("imagen");

            entity.HasOne(d => d.ConductorIdconductorNavigation).WithOne(p => p.FotoConductor)
                .HasForeignKey<FotoConductor>(d => d.ConductorIdconductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_table1_conductor1");
        });

        modelBuilder.Entity<Fotografium>(entity =>
        {
            entity.HasKey(e => e.UsuarioIdusuario).HasName("PRIMARY");

            entity.ToTable("fotografia");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_table1_usuario1_idx");

            entity.Property(e => e.UsuarioIdusuario)
                .ValueGeneratedNever()
                .HasColumnName("usuario_idusuario");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Imagen).HasColumnName("imagen");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithOne(p => p.Fotografium)
                .HasForeignKey<Fotografium>(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_table1_usuario1");
        });

        modelBuilder.Entity<LiquidacionCabecera>(entity =>
        {
            entity.HasKey(e => e.IdLiquidacion).HasName("PRIMARY");

            entity.ToTable("liquidacion_cabecera");

            entity.HasIndex(e => e.Idconductor, "fk_liquidacion_cabecera_conductor_idx");

            entity.Property(e => e.IdLiquidacion).HasColumnName("id_liquidacion");
            entity.Property(e => e.FechaEmision)
                .HasColumnType("datetime")
                .HasColumnName("fecha_emision");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInico)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inico");
            entity.Property(e => e.Idconductor).HasColumnName("idconductor");
            entity.Property(e => e.MontoComision)
                .HasPrecision(10, 2)
                .HasColumnName("monto_comision");
            entity.Property(e => e.MontoPagado)
                .HasPrecision(10, 2)
                .HasColumnName("monto_pagado");

            entity.HasOne(d => d.IdconductorNavigation).WithMany(p => p.LiquidacionCabeceras)
                .HasForeignKey(d => d.Idconductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_liquidacion_cabecera_conductor");
        });

        modelBuilder.Entity<LiquidacionDetalle>(entity =>
        {
            entity.HasKey(e => new { e.IdLiquidacion, e.IdCorridaAsignacion })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("liquidacion_detalle");

            entity.HasIndex(e => e.IdCorridaAsignacion, "fk_liquidacion_corrida_asignacion_idx");

            entity.HasIndex(e => e.IdLiquidacion, "fk_liquidacion_liquidacion_cabecera_idx");

            entity.Property(e => e.IdLiquidacion)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_liquidacion");
            entity.Property(e => e.IdCorridaAsignacion).HasColumnName("id_corrida_asignacion");
            entity.Property(e => e.Logical).HasColumnName("logical");

            entity.HasOne(d => d.IdCorridaAsignacionNavigation).WithMany(p => p.LiquidacionDetalles)
                .HasForeignKey(d => d.IdCorridaAsignacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_liquidacion_corrida_asignacion");

            entity.HasOne(d => d.IdLiquidacionNavigation).WithMany(p => p.LiquidacionDetalles)
                .HasForeignKey(d => d.IdLiquidacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_liquidacion_liquidacion_cabecera");
        });

        modelBuilder.Entity<Logcancelacionviaje>(entity =>
        {
            entity.HasKey(e => e.Idlogcancelacionviajes).HasName("PRIMARY");

            entity.ToTable("logcancelacionviajes");

            entity.HasIndex(e => e.Idlogcancelacionviajes, "fk_idlogcancelacionviajes_idx");

            entity.HasIndex(e => e.ViajeIdviaje, "fk_logcancelacionviajes_viaje_idx");

            entity.Property(e => e.Idlogcancelacionviajes).HasColumnName("idlogcancelacionviajes");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Saldoabonadaamonedero)
                .HasPrecision(17, 2)
                .HasColumnName("saldoabonadaamonedero");
            entity.Property(e => e.ViajeIdviaje).HasColumnName("viaje_idviaje");

            entity.HasOne(d => d.ViajeIdviajeNavigation).WithMany(p => p.Logcancelacionviajes)
                .HasForeignKey(d => d.ViajeIdviaje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_logcancelacionviajes_viaje");
        });

        modelBuilder.Entity<Logerrorfactura>(entity =>
        {
            entity.HasKey(e => e.Idlogerrorfactura).HasName("PRIMARY");

            entity.ToTable("logerrorfactura");

            entity.HasIndex(e => e.Idlogerrorfactura, "fk_idlogerrorfactura_idx");

            entity.Property(e => e.Idlogerrorfactura).HasColumnName("idlogerrorfactura");
            entity.Property(e => e.Descripcionerror)
                .HasColumnType("mediumtext")
                .HasColumnName("descripcionerror");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Respuestaservicio)
                .HasColumnType("mediumtext")
                .HasColumnName("respuestaservicio");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");
            entity.Property(e => e.Xmlrequest)
                .HasColumnType("mediumtext")
                .HasColumnName("xmlrequest");
        });

        modelBuilder.Entity<Logtransaccion>(entity =>
        {
            entity.HasKey(e => e.Idlogtransaccion).HasName("PRIMARY");

            entity.ToTable("logtransaccion");

            entity.Property(e => e.Idlogtransaccion).HasColumnName("idlogtransaccion");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Respuesta)
                .HasMaxLength(2000)
                .HasColumnName("respuesta");
            entity.Property(e => e.TransaccionNumeroordenpay)
                .HasMaxLength(36)
                .HasColumnName("transaccion_numeroordenpay");
        });

        modelBuilder.Entity<Logtransaccionesentidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("logtransaccionesentidades");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accion)
                .HasMaxLength(100)
                .HasColumnName("accion");
            entity.Property(e => e.Fecha)
                .HasMaxLength(100)
                .HasColumnName("fecha");
            entity.Property(e => e.Identidad)
                .HasMaxLength(100)
                .HasColumnName("identidad");
            entity.Property(e => e.Nombreentidad)
                .HasMaxLength(100)
                .HasColumnName("nombreentidad");
            entity.Property(e => e.Usuario)
                .HasMaxLength(100)
                .HasColumnName("usuario");
            entity.Property(e => e.Valoranterior)
                .HasMaxLength(250)
                .HasColumnName("valoranterior");
            entity.Property(e => e.Valornuevo)
                .HasMaxLength(250)
                .HasColumnName("valornuevo");
        });

        modelBuilder.Entity<Mantenimiento>(entity =>
        {
            entity.HasKey(e => e.Idmantenimiento).HasName("PRIMARY");

            entity.ToTable("mantenimiento");

            entity.HasIndex(e => e.CausamantenimientoIdcausamantenimiento, "fk_mantenimiento_causamantenimiento");

            entity.HasIndex(e => e.DepositoIddeposito, "fk_mantenimiento_deposito");

            entity.HasIndex(e => e.MantenimientodetalleIdmantenimientodetalle, "fk_mantenimiento_mantenimientodetalle");

            entity.HasIndex(e => e.MecanicoIdmecanico, "fk_mantenimiento_mecanico");

            entity.HasIndex(e => e.PreventivoIdpreventivo, "fk_mantenimiento_preventivo");

            entity.HasIndex(e => e.TiposervicioIdtiposervicio, "fk_mantenimiento_tiposervicio");

            entity.HasIndex(e => e.VehiculoIdvehiculo, "fk_mantenimiento_vehiculo");

            entity.Property(e => e.Idmantenimiento).HasColumnName("idmantenimiento");
            entity.Property(e => e.CausamantenimientoIdcausamantenimiento).HasColumnName("causamantenimiento_idcausamantenimiento");
            entity.Property(e => e.DepositoIddeposito).HasColumnName("deposito_iddeposito");
            entity.Property(e => e.DepositoVehiculo)
                .HasMaxLength(100)
                .HasColumnName("deposito_vehiculo");
            entity.Property(e => e.DesasignaVehiculo)
                .HasMaxLength(100)
                .HasColumnName("desasigna_vehiculo");
            entity.Property(e => e.FechaCaptura)
                .HasMaxLength(100)
                .HasColumnName("fecha_captura");
            entity.Property(e => e.FechaProgramada)
                .HasMaxLength(100)
                .HasColumnName("fecha_programada");
            entity.Property(e => e.KmsEntrada)
                .HasMaxLength(100)
                .HasColumnName("kms_entrada");
            entity.Property(e => e.MantenimientodetalleIdmantenimientodetalle).HasColumnName("mantenimientodetalle_idmantenimientodetalle");
            entity.Property(e => e.MecanicoIdmecanico).HasColumnName("mecanico_idmecanico");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(100)
                .HasColumnName("observaciones");
            entity.Property(e => e.PreventivoIdpreventivo).HasColumnName("preventivo_idpreventivo");
            entity.Property(e => e.Tanque)
                .HasMaxLength(100)
                .HasColumnName("tanque");
            entity.Property(e => e.TiposervicioIdtiposervicio).HasColumnName("tiposervicio_idtiposervicio");
            entity.Property(e => e.VehiculoIdvehiculo).HasColumnName("vehiculo_idvehiculo");

            entity.HasOne(d => d.CausamantenimientoIdcausamantenimientoNavigation).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.CausamantenimientoIdcausamantenimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mantenimiento_causamantenimiento");

            entity.HasOne(d => d.DepositoIddepositoNavigation).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.DepositoIddeposito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mantenimiento_deposito");

            entity.HasOne(d => d.MantenimientodetalleIdmantenimientodetalleNavigation).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.MantenimientodetalleIdmantenimientodetalle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mantenimiento_mantenimientodetalle");

            entity.HasOne(d => d.MecanicoIdmecanicoNavigation).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.MecanicoIdmecanico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mantenimiento_mecanico");

            entity.HasOne(d => d.PreventivoIdpreventivoNavigation).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.PreventivoIdpreventivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mantenimiento_preventivo");

            entity.HasOne(d => d.TiposervicioIdtiposervicioNavigation).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.TiposervicioIdtiposervicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mantenimiento_tiposervicio");

            entity.HasOne(d => d.VehiculoIdvehiculoNavigation).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.VehiculoIdvehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mantenimiento_vehiculo");
        });

        modelBuilder.Entity<MantenimientoDetalle>(entity =>
        {
            entity.HasKey(e => e.Idmantenimientodetalle).HasName("PRIMARY");

            entity.ToTable("mantenimiento_detalle");

            entity.Property(e => e.Idmantenimientodetalle).HasColumnName("idmantenimientodetalle");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Referencia)
                .HasMaxLength(100)
                .HasColumnName("referencia");
            entity.Property(e => e.Reparacion)
                .HasMaxLength(100)
                .HasColumnName("reparacion");
            entity.Property(e => e.Valor).HasColumnName("valor");
        });

        modelBuilder.Entity<Mecanico>(entity =>
        {
            entity.HasKey(e => e.IdMecanico).HasName("PRIMARY");

            entity.ToTable("mecanico");

            entity.HasIndex(e => e.IdDeposito, "fk_mecanico_deposito");

            entity.Property(e => e.IdMecanico).HasColumnName("id_mecanico");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fecha_ingreso");
            entity.Property(e => e.IdDeposito).HasColumnName("id_deposito");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Numero)
                .HasMaxLength(100)
                .HasColumnName("numero");

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.Mecanicos)
                .HasForeignKey(d => d.IdDeposito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mecanico_deposito");
        });

        modelBuilder.Entity<Membresium>(entity =>
        {
            entity.HasKey(e => e.Idmembreria).HasName("PRIMARY");

            entity.ToTable("membresia");

            entity.HasIndex(e => e.TransaccionIdtransaccion, "fk_membresia_transaccion_idx");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_usuario_membresia_idx");

            entity.Property(e => e.Idmembreria).HasColumnName("idmembreria");
            entity.Property(e => e.Desde)
                .HasColumnType("datetime")
                .HasColumnName("desde");
            entity.Property(e => e.Hasta)
                .HasColumnType("datetime")
                .HasColumnName("hasta");
            entity.Property(e => e.TransaccionIdtransaccion).HasColumnName("transaccion_idtransaccion");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");

            entity.HasOne(d => d.TransaccionIdtransaccionNavigation).WithMany(p => p.Membresia)
                .HasForeignKey(d => d.TransaccionIdtransaccion)
                .HasConstraintName("fk_membresia_transaccion");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.Membresia)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .HasConstraintName("fk_usuario_membresia");
        });

        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.HasKey(e => e.MensajeId).HasName("PRIMARY");

            entity.ToTable("mensaje");

            entity.HasIndex(e => e.MensajeId, "MensajeId_UNIQUE").IsUnique();

            entity.Property(e => e.Comentario).HasColumnType("text");
            entity.Property(e => e.FechaHoraCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Idmenu).HasName("PRIMARY");

            entity.ToTable("menu");

            entity.HasIndex(e => e.MenuIdpadre, "fk_menu_menu1_idx");

            entity.Property(e => e.Idmenu).HasColumnName("idmenu");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Icono)
                .HasMaxLength(100)
                .HasColumnName("icono");
            entity.Property(e => e.MenuIdpadre).HasColumnName("menu_idpadre");
            entity.Property(e => e.Ruta)
                .HasMaxLength(120)
                .HasColumnName("ruta");
            entity.Property(e => e.Titulo)
                .HasMaxLength(45)
                .HasColumnName("titulo");

            entity.HasOne(d => d.MenuIdpadreNavigation).WithMany(p => p.InverseMenuIdpadreNavigation)
                .HasForeignKey(d => d.MenuIdpadre)
                .HasConstraintName("fk_menu_menu1");
        });

        modelBuilder.Entity<Metodopago>(entity =>
        {
            entity.HasKey(e => e.Idmetodopago).HasName("PRIMARY");

            entity.ToTable("metodopago");

            entity.Property(e => e.Idmetodopago)
                .ValueGeneratedNever()
                .HasColumnName("idmetodopago");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Mfaportal>(entity =>
        {
            entity.HasKey(e => e.AdminIdadmin).HasName("PRIMARY");

            entity.ToTable("mfaportal");

            entity.HasIndex(e => e.AdminIdadmin, "fk_MFAPortal_admin1_idx");

            entity.Property(e => e.AdminIdadmin)
                .ValueGeneratedNever()
                .HasColumnName("admin_idadmin");
            entity.Property(e => e.Codigo)
                .HasMaxLength(4)
                .HasColumnName("codigo");
            entity.Property(e => e.Expira)
                .HasColumnType("datetime")
                .HasColumnName("expira");

            entity.HasOne(d => d.AdminIdadminNavigation).WithOne(p => p.Mfaportal)
                .HasForeignKey<Mfaportal>(d => d.AdminIdadmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_MFAPortal_admin1");
        });

        modelBuilder.Entity<Monedero>(entity =>
        {
            entity.HasKey(e => e.UsuarioIdusuario).HasName("PRIMARY");

            entity.ToTable("monedero");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_monedero_usuario1_idx");

            entity.Property(e => e.UsuarioIdusuario)
                .ValueGeneratedNever()
                .HasColumnName("usuario_idusuario");
            entity.Property(e => e.Saldo)
                .HasPrecision(17, 2)
                .HasColumnName("saldo");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithOne(p => p.Monedero)
                .HasForeignKey<Monedero>(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_monedero_usuario1");
        });

        modelBuilder.Entity<MotivoAuxilioVial>(entity =>
        {
            entity.HasKey(e => e.Idmotivo).HasName("PRIMARY");

            entity.ToTable("motivo_auxilio_vial");

            entity.Property(e => e.Idmotivo).HasColumnName("idmotivo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<NotificacionesSiniestro>(entity =>
        {
            entity.HasKey(e => e.IdNotifsini).HasName("PRIMARY");

            entity.ToTable("notificaciones_siniestro");

            entity.HasIndex(e => e.SiniestroIdsiniestro, "fk_NotificacionesSiniestros_Siniestros");

            entity.Property(e => e.IdNotifsini).HasColumnName("id_notifsini");
            entity.Property(e => e.Asunto)
                .HasMaxLength(250)
                .HasColumnName("asunto");
            entity.Property(e => e.DestinatarioNombre)
                .HasMaxLength(150)
                .HasColumnName("destinatario_nombre");
            entity.Property(e => e.EnviadoPor)
                .HasMaxLength(100)
                .HasColumnName("enviado_por");
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecha_envio");
            entity.Property(e => e.MedioContacto)
                .HasMaxLength(100)
                .HasColumnName("medio_contacto");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");
            entity.Property(e => e.SiniestroIdsiniestro).HasColumnName("siniestro_idsiniestro");
            entity.Property(e => e.TipoNotificacion)
                .HasMaxLength(50)
                .HasColumnName("tipo_notificacion");

            entity.HasOne(d => d.SiniestroIdsiniestroNavigation).WithMany(p => p.NotificacionesSiniestros)
                .HasForeignKey(d => d.SiniestroIdsiniestro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_NotificacionesSiniestros_Siniestros");
        });

        modelBuilder.Entity<Paradum>(entity =>
        {
            entity.HasKey(e => e.Idparada).HasName("PRIMARY");

            entity.ToTable("parada");

            entity.Property(e => e.Idparada).HasColumnName("idparada");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Calle)
                .HasMaxLength(250)
                .HasColumnName("calle");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(250)
                .HasColumnName("ciudad");
            entity.Property(e => e.Codigopostal)
                .HasMaxLength(10)
                .HasColumnName("codigopostal");
            entity.Property(e => e.Colonia)
                .HasMaxLength(250)
                .HasColumnName("colonia");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Latitud)
                .HasPrecision(12, 8)
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasPrecision(12, 8)
                .HasColumnName("longitud");
            entity.Property(e => e.Municipio)
                .HasMaxLength(250)
                .HasColumnName("municipio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
            entity.Property(e => e.Numero)
                .HasMaxLength(100)
                .HasColumnName("numero");
            entity.Property(e => e.Orden).HasColumnName("orden");
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.HasKey(e => e.Idperfil).HasName("PRIMARY");

            entity.ToTable("perfil");

            entity.Property(e => e.Idperfil).HasColumnName("idperfil");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => new { e.PerfilIdperfil, e.MenuIdmenu })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("permiso");

            entity.HasIndex(e => e.MenuIdmenu, "fk_permiso_menu1_idx");

            entity.Property(e => e.PerfilIdperfil).HasColumnName("perfil_idperfil");
            entity.Property(e => e.MenuIdmenu).HasColumnName("menu_idmenu");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Agregar)
                .HasColumnType("bit(1)")
                .HasColumnName("agregar");
            entity.Property(e => e.Ejecutar)
                .HasColumnType("bit(1)")
                .HasColumnName("ejecutar");
            entity.Property(e => e.Eliminar)
                .HasColumnType("bit(1)")
                .HasColumnName("eliminar");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Modificar)
                .HasColumnType("bit(1)")
                .HasColumnName("modificar");

            entity.HasOne(d => d.MenuIdmenuNavigation).WithMany(p => p.Permisos)
                .HasForeignKey(d => d.MenuIdmenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_permiso_menu1");

            entity.HasOne(d => d.PerfilIdperfilNavigation).WithMany(p => p.Permisos)
                .HasForeignKey(d => d.PerfilIdperfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_perfil_perfil1");
        });

        modelBuilder.Entity<Personalidadjuridica>(entity =>
        {
            entity.HasKey(e => e.Idpersonalidadjuridica).HasName("PRIMARY");

            entity.ToTable("personalidadjuridica");

            entity.Property(e => e.Idpersonalidadjuridica).HasColumnName("idpersonalidadjuridica");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Politicascompensacion>(entity =>
        {
            entity.HasKey(e => e.Idpoliticacompensacion).HasName("PRIMARY");

            entity.ToTable("politicascompensacion");

            entity.HasIndex(e => e.EmpresaIdempresa, "politicascompensacion_empresa_FK");

            entity.Property(e => e.Idpoliticacompensacion).HasColumnName("idpoliticacompensacion");
            entity.Property(e => e.Activa)
                .HasColumnType("bit(1)")
                .HasColumnName("activa");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Porcentajecompensacion)
                .HasPrecision(10, 2)
                .HasColumnName("porcentajecompensacion");
            entity.Property(e => e.Rangotiempo)
                .HasPrecision(10, 2)
                .HasColumnName("rangotiempo");
            entity.Property(e => e.Tipocancelacion).HasColumnName("tipocancelacion");
            entity.Property(e => e.Tipopolitica).HasColumnName("tipopolitica");
            entity.Property(e => e.Tipotiempo).HasColumnName("tipotiempo");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.Politicascompensacions)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .HasConstraintName("politicascompensacion_empresa_FK");
        });

        modelBuilder.Entity<Preventivo>(entity =>
        {
            entity.HasKey(e => e.Idpreventivo).HasName("PRIMARY");

            entity.ToTable("preventivo");

            entity.HasIndex(e => e.TiposervicioIdtiposervicio, "fk_preventivo_tiposervicio");

            entity.Property(e => e.Idpreventivo).HasColumnName("idpreventivo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Proyecta)
                .HasMaxLength(10)
                .HasColumnName("proyecta");
            entity.Property(e => e.TiposervicioIdtiposervicio).HasColumnName("tiposervicio_idtiposervicio");

            entity.HasOne(d => d.TiposervicioIdtiposervicioNavigation).WithMany(p => p.Preventivos)
                .HasForeignKey(d => d.TiposervicioIdtiposervicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_preventivo_tiposervicio");
        });

        modelBuilder.Entity<Promocion>(entity =>
        {
            entity.HasKey(e => e.Idpromocion).HasName("PRIMARY");

            entity.ToTable("promocion");

            entity.HasIndex(e => e.EmpresaIdempresa, "fk_promocion_empresa1_idx");

            entity.HasIndex(e => e.TipodescuentoIdtipodescuento, "fk_promocion_tipodescuento1_idx");

            entity.HasIndex(e => e.TipopromocionIdtipopromocion, "fk_promocion_tipopromocion1_idx");

            entity.Property(e => e.Idpromocion).HasColumnName("idpromocion");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
            entity.Property(e => e.TipodescuentoIdtipodescuento).HasColumnName("tipodescuento_idtipodescuento");
            entity.Property(e => e.TipopromocionIdtipopromocion).HasColumnName("tipopromocion_idtipopromocion");
            entity.Property(e => e.Vigenciadesde)
                .HasColumnType("datetime")
                .HasColumnName("vigenciadesde");
            entity.Property(e => e.Vigenciahasta)
                .HasColumnType("datetime")
                .HasColumnName("vigenciahasta");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.Promocions)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .HasConstraintName("fk_promocion_empresa1");

            entity.HasOne(d => d.TipodescuentoIdtipodescuentoNavigation).WithMany(p => p.Promocions)
                .HasForeignKey(d => d.TipodescuentoIdtipodescuento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_promocion_tipodescuento1");

            entity.HasOne(d => d.TipopromocionIdtipopromocionNavigation).WithMany(p => p.Promocions)
                .HasForeignKey(d => d.TipopromocionIdtipopromocion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_promocion_tipopromocion1");
        });

        modelBuilder.Entity<PromocionCorridum>(entity =>
        {
            entity.HasKey(e => e.PromocionCorridaid).HasName("PRIMARY");

            entity.ToTable("promocion_corrida");

            entity.HasIndex(e => e.CorridaIdcorrida, "fk_promocion_corrida_corrida1_idx");

            entity.HasIndex(e => e.PromocionIdpromocion, "fk_promocion_corrida_promocion1_idx");

            entity.Property(e => e.PromocionCorridaid).HasColumnName("promocion_corridaid");
            entity.Property(e => e.CorridaIdcorrida).HasColumnName("corrida_idcorrida");
            entity.Property(e => e.PromocionIdpromocion).HasColumnName("promocion_idpromocion");

            entity.HasOne(d => d.CorridaIdcorridaNavigation).WithMany(p => p.PromocionCorrida)
                .HasForeignKey(d => d.CorridaIdcorrida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_promocion_corrida_corrida1");

            entity.HasOne(d => d.PromocionIdpromocionNavigation).WithMany(p => p.PromocionCorrida)
                .HasForeignKey(d => d.PromocionIdpromocion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_promocion_corrida_promocion1");
        });

        modelBuilder.Entity<PromocionEmpresa>(entity =>
        {
            entity.HasKey(e => new { e.PromocionIdpromocion, e.EmpresaIdempresa })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("promocion_empresa");

            entity.HasIndex(e => e.EmpresaIdempresa, "fk_promocionempresa_empresa1_idx");

            entity.HasIndex(e => e.PromocionIdpromocion, "fk_promocionempresa_promocion1_idx");

            entity.Property(e => e.PromocionIdpromocion).HasColumnName("promocion_idpromocion");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.PromocionEmpresas)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_promocionempresa_empresa1");

            entity.HasOne(d => d.PromocionIdpromocionNavigation).WithMany(p => p.PromocionEmpresas)
                .HasForeignKey(d => d.PromocionIdpromocion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_promocionempresa_promocion1");
        });

        modelBuilder.Entity<PromocionHorario>(entity =>
        {
            entity.HasKey(e => e.IdpromocionHorario).HasName("PRIMARY");

            entity.ToTable("promocion_horario");

            entity.HasIndex(e => e.PromocionIdpromocion, "fk_promocion_horario_promocion1_idx");

            entity.Property(e => e.IdpromocionHorario).HasColumnName("idpromocion_horario");
            entity.Property(e => e.Horadesde)
                .HasColumnType("time")
                .HasColumnName("horadesde");
            entity.Property(e => e.Horahasta)
                .HasColumnType("time")
                .HasColumnName("horahasta");
            entity.Property(e => e.PromocionIdpromocion).HasColumnName("promocion_idpromocion");

            entity.HasOne(d => d.PromocionIdpromocionNavigation).WithMany(p => p.PromocionHorarios)
                .HasForeignKey(d => d.PromocionIdpromocion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_promocion_horario_promocion1");
        });

        modelBuilder.Entity<PromocionRutum>(entity =>
        {
            entity.HasKey(e => new { e.PromocionIdpromocion, e.RutaIdruta })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("promocion_ruta");

            entity.HasIndex(e => e.PromocionIdpromocion, "fk_promocion_ruta_promocion1_idx");

            entity.HasIndex(e => e.RutaIdruta, "fk_promocion_ruta_ruta1_idx");

            entity.Property(e => e.PromocionIdpromocion).HasColumnName("promocion_idpromocion");
            entity.Property(e => e.RutaIdruta).HasColumnName("ruta_idruta");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");

            entity.HasOne(d => d.PromocionIdpromocionNavigation).WithMany(p => p.PromocionRuta)
                .HasForeignKey(d => d.PromocionIdpromocion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_promocion_ruta_promocion1");

            entity.HasOne(d => d.RutaIdrutaNavigation).WithMany(p => p.PromocionRuta)
                .HasForeignKey(d => d.RutaIdruta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_promocion_ruta_ruta1");
        });

        modelBuilder.Entity<Puntovirtual>(entity =>
        {
            entity.HasKey(e => e.Idpuntovirtual).HasName("PRIMARY");

            entity.ToTable("puntovirtual");

            entity.HasIndex(e => new { e.RutaParadaRutaIdruta, e.RutaParadaParadaIdparada }, "fk_puntovirtual_ruta_estacion1_idx");

            entity.Property(e => e.Idpuntovirtual).HasColumnName("idpuntovirtual");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Latitud)
                .HasPrecision(12, 8)
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasPrecision(12, 8)
                .HasColumnName("longitud");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.RutaParadaParadaIdparada).HasColumnName("ruta_parada_parada_idparada");
            entity.Property(e => e.RutaParadaRutaIdruta).HasColumnName("ruta_parada_ruta_idruta");

            entity.HasOne(d => d.RutaParada).WithMany(p => p.Puntovirtuals)
                .HasForeignKey(d => new { d.RutaParadaRutaIdruta, d.RutaParadaParadaIdparada })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_puntovirtual_ruta_estacion1");
        });

        modelBuilder.Entity<Recarga>(entity =>
        {
            entity.HasKey(e => e.Idrecarga).HasName("PRIMARY");

            entity.ToTable("recarga");

            entity.HasIndex(e => e.MetodopagoIdmetodopago, "fk_recarga_metodopago1_idx");

            entity.HasIndex(e => e.MonederoUsuarioIdusuario, "fk_recarga_monedero1_idx");

            entity.Property(e => e.Idrecarga).HasColumnName("idrecarga");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.MetodopagoIdmetodopago).HasColumnName("metodopago_idmetodopago");
            entity.Property(e => e.MonederoUsuarioIdusuario).HasColumnName("monedero_usuario_idusuario");

            entity.HasOne(d => d.MetodopagoIdmetodopagoNavigation).WithMany(p => p.Recargas)
                .HasForeignKey(d => d.MetodopagoIdmetodopago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recarga_metodopago1");

            entity.HasOne(d => d.MonederoUsuarioIdusuarioNavigation).WithMany(p => p.Recargas)
                .HasForeignKey(d => d.MonederoUsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recarga_monedero1");
        });

        modelBuilder.Entity<Regimenfiscalreceptor>(entity =>
        {
            entity.HasKey(e => e.Idregimenfiscalreceptor).HasName("PRIMARY");

            entity.ToTable("regimenfiscalreceptor");

            entity.HasIndex(e => e.Clave, "clave_UNIQUE").IsUnique();

            entity.Property(e => e.Idregimenfiscalreceptor).HasColumnName("idregimenfiscalreceptor");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Clave)
                .HasMaxLength(10)
                .HasColumnName("clave");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Fisica)
                .HasColumnType("bit(1)")
                .HasColumnName("fisica");
            entity.Property(e => e.Moral)
                .HasColumnType("bit(1)")
                .HasColumnName("moral");
        });

        modelBuilder.Entity<RutaParadum>(entity =>
        {
            entity.HasKey(e => new { e.RutaIdruta, e.ParadaIdparada })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("ruta_parada");

            entity.HasIndex(e => e.TipoestacionIdtipoestacion, "fk_rutaparada_tipoestacion_idx");

            entity.HasIndex(e => e.ParadaIdparada, "fk_table1_estacion1_idx");

            entity.HasIndex(e => e.RutaIdruta, "fk_table1_ruta2_idx");

            entity.Property(e => e.RutaIdruta).HasColumnName("ruta_idruta");
            entity.Property(e => e.ParadaIdparada).HasColumnName("parada_idparada");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.Tiemposeg).HasColumnName("tiemposeg");
            entity.Property(e => e.TipoestacionIdtipoestacion).HasColumnName("tipoestacion_idtipoestacion");

            entity.HasOne(d => d.ParadaIdparadaNavigation).WithMany(p => p.RutaParada)
                .HasForeignKey(d => d.ParadaIdparada)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_table1_estacion1");

            entity.HasOne(d => d.RutaIdrutaNavigation).WithMany(p => p.RutaParada)
                .HasForeignKey(d => d.RutaIdruta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_table1_ruta2");

            entity.HasOne(d => d.TipoestacionIdtipoestacionNavigation).WithMany(p => p.RutaParada)
                .HasForeignKey(d => d.TipoestacionIdtipoestacion)
                .HasConstraintName("fk_rutaparada_tipoestacion");
        });

        modelBuilder.Entity<Rutum>(entity =>
        {
            entity.HasKey(e => e.Idruta).HasName("PRIMARY");

            entity.ToTable("ruta");

            entity.HasIndex(e => e.EmpresaIdempresa, "ruta_FK");

            entity.HasIndex(e => e.TipotarifaIdtipotarifa, "tipotarifa_idx");

            entity.Property(e => e.Idruta).HasColumnName("idruta");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Distanciamts).HasColumnName("distanciamts");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Googlemapsruta).HasColumnName("googlemapsruta");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
            entity.Property(e => e.TipotarifaIdtipotarifa)
                .HasDefaultValueSql("'1'")
                .HasColumnName("tipotarifa_idtipotarifa");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.Ruta)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .HasConstraintName("ruta_FK");

            entity.HasOne(d => d.TipotarifaIdtipotarifaNavigation).WithMany(p => p.Ruta)
                .HasForeignKey(d => d.TipotarifaIdtipotarifa)
                .HasConstraintName("tipotarifa");
        });

        modelBuilder.Entity<Siniestro>(entity =>
        {
            entity.HasKey(e => e.Idsiniestro).HasName("PRIMARY");

            entity.ToTable("siniestros");

            entity.HasIndex(e => e.ConductorIdconductor, "fk_siniestro_conductor");

            entity.HasIndex(e => e.DepositoIdIdDeposito, "fk_siniestro_deposito");

            entity.HasIndex(e => e.FallaAuxilioIdIdfalla, "fk_siniestro_falla");

            entity.HasIndex(e => e.MotivoAuxilioIdIdmotivo, "fk_siniestro_motivo");

            entity.HasIndex(e => e.VehiculoIdvehiculo, "fk_siniestro_vehiculo");

            entity.Property(e => e.Idsiniestro).HasColumnName("idsiniestro");
            entity.Property(e => e.CoberturaDanos)
                .HasPrecision(10, 2)
                .HasColumnName("cobertura_danos");
            entity.Property(e => e.ConductorIdconductor).HasColumnName("conductor_idconductor");
            entity.Property(e => e.CulpaOperador).HasColumnName("culpa_operador");
            entity.Property(e => e.DepositoIdIdDeposito).HasColumnName("deposito_id_id_deposito");
            entity.Property(e => e.Estatus)
                .HasMaxLength(50)
                .HasColumnName("estatus");
            entity.Property(e => e.Facturado).HasColumnName("facturado");
            entity.Property(e => e.FallaAuxilioIdIdfalla).HasColumnName("falla_auxilio_id_idfalla");
            entity.Property(e => e.FechaReporte).HasColumnName("fecha_reporte");
            entity.Property(e => e.Fecharegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.HoraReporte)
                .HasColumnType("time")
                .HasColumnName("hora_reporte");
            entity.Property(e => e.LugarSiniestro)
                .HasMaxLength(200)
                .HasColumnName("lugar_siniestro");
            entity.Property(e => e.MotivoAuxilioIdIdmotivo).HasColumnName("motivo_auxilio_id_idmotivo");
            entity.Property(e => e.NumeroAuxilio)
                .HasMaxLength(50)
                .HasColumnName("numero_auxilio");
            entity.Property(e => e.Orden)
                .HasMaxLength(50)
                .HasColumnName("orden");
            entity.Property(e => e.ReporteAtencion)
                .HasColumnType("text")
                .HasColumnName("reporte_atencion");
            entity.Property(e => e.ReporteConclusiones)
                .HasColumnType("text")
                .HasColumnName("reporte_conclusiones");
            entity.Property(e => e.ReporteOperador)
                .HasColumnType("text")
                .HasColumnName("reporte_operador");
            entity.Property(e => e.ResponsabilidadCompartida).HasColumnName("responsabilidad_compartida");
            entity.Property(e => e.TipoPoliza)
                .HasMaxLength(50)
                .HasColumnName("tipo_poliza");
            entity.Property(e => e.VehiculoIdvehiculo).HasColumnName("vehiculo_idvehiculo");
            entity.Property(e => e.VencimientoPoliza).HasColumnName("vencimiento_poliza");

            entity.HasOne(d => d.ConductorIdconductorNavigation).WithMany(p => p.Siniestros)
                .HasForeignKey(d => d.ConductorIdconductor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_siniestro_conductor");

            entity.HasOne(d => d.DepositoIdIdDepositoNavigation).WithMany(p => p.Siniestros)
                .HasForeignKey(d => d.DepositoIdIdDeposito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_siniestro_deposito");

            entity.HasOne(d => d.FallaAuxilioIdIdfallaNavigation).WithMany(p => p.Siniestros)
                .HasForeignKey(d => d.FallaAuxilioIdIdfalla)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_siniestro_falla");

            entity.HasOne(d => d.MotivoAuxilioIdIdmotivoNavigation).WithMany(p => p.Siniestros)
                .HasForeignKey(d => d.MotivoAuxilioIdIdmotivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_siniestro_motivo");

            entity.HasOne(d => d.VehiculoIdvehiculoNavigation).WithMany(p => p.Siniestros)
                .HasForeignKey(d => d.VehiculoIdvehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_siniestro_vehiculo");
        });

        modelBuilder.Entity<Taller>(entity =>
        {
            entity.HasKey(e => e.IdTaller).HasName("PRIMARY");

            entity.ToTable("taller");

            entity.HasIndex(e => e.ZonaIdzona, "fk_taller_zona");

            entity.Property(e => e.IdTaller).HasColumnName("id_taller");
            entity.Property(e => e.NombreTaller)
                .HasMaxLength(255)
                .HasColumnName("nombre_taller");
            entity.Property(e => e.ZonaIdzona).HasColumnName("zona_idzona");

            entity.HasOne(d => d.ZonaIdzonaNavigation).WithMany(p => p.Tallers)
                .HasForeignKey(d => d.ZonaIdzona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_taller_zona");
        });

        modelBuilder.Entity<TarifaEscalonadum>(entity =>
        {
            entity.HasKey(e => new { e.ParadaIdparada, e.ParadaIdparada1, e.EmpresaIdempresa, e.RutaIdruta })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

            entity.ToTable("tarifa_escalonada");

            entity.HasIndex(e => e.ParadaIdparada, "fk_tarifa_parada1_idx");

            entity.HasIndex(e => e.ParadaIdparada1, "fk_tarifa_parada2_idx");

            entity.Property(e => e.ParadaIdparada).HasColumnName("parada_idparada");
            entity.Property(e => e.ParadaIdparada1).HasColumnName("parada_idparada1");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.RutaIdruta).HasColumnName("ruta_idruta");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");

            entity.HasOne(d => d.ParadaIdparadaNavigation).WithMany(p => p.TarifaEscalonadumParadaIdparadaNavigations)
                .HasForeignKey(d => d.ParadaIdparada)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tarifa_parada1");

            entity.HasOne(d => d.ParadaIdparada1Navigation).WithMany(p => p.TarifaEscalonadumParadaIdparada1Navigations)
                .HasForeignKey(d => d.ParadaIdparada1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tarifa_parada2");
        });

        modelBuilder.Entity<TarifaGeneral>(entity =>
        {
            entity.HasKey(e => e.RutaIdruta).HasName("PRIMARY");

            entity.ToTable("tarifa_general");

            entity.HasIndex(e => new { e.EmpresaIdempresa, e.RutaIdruta }, "fk_tarifa_general_empresa_ruta1_idx");

            entity.Property(e => e.RutaIdruta)
                .ValueGeneratedNever()
                .HasColumnName("ruta_idruta");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");

            entity.HasOne(d => d.RutaIdrutaNavigation).WithOne(p => p.TarifaGeneral)
                .HasForeignKey<TarifaGeneral>(d => d.RutaIdruta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tarifa_general_ruta_FK");
        });

        modelBuilder.Entity<TipoServicio>(entity =>
        {
            entity.HasKey(e => e.IdTiposervicio).HasName("PRIMARY");

            entity.ToTable("tipo_servicio");

            entity.Property(e => e.IdTiposervicio).HasColumnName("id_tiposervicio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipocodigodescuento>(entity =>
        {
            entity.HasKey(e => e.Idtipocodigodescuento).HasName("PRIMARY");

            entity.ToTable("tipocodigodescuento");

            entity.Property(e => e.Idtipocodigodescuento).HasColumnName("idtipocodigodescuento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipocontenido>(entity =>
        {
            entity.HasKey(e => e.Idtipocontenido).HasName("PRIMARY");

            entity.ToTable("tipocontenido");

            entity.Property(e => e.Idtipocontenido)
                .ValueGeneratedNever()
                .HasColumnName("idtipocontenido");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipodescuento>(entity =>
        {
            entity.HasKey(e => e.Idtipodescuento).HasName("PRIMARY");

            entity.ToTable("tipodescuento");

            entity.Property(e => e.Idtipodescuento)
                .ValueGeneratedNever()
                .HasColumnName("idtipodescuento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipoestacion>(entity =>
        {
            entity.HasKey(e => e.Idtipoestacion).HasName("PRIMARY");

            entity.ToTable("tipoestacion");

            entity.Property(e => e.Idtipoestacion)
                .ValueGeneratedNever()
                .HasColumnName("idtipoestacion");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipofavorito>(entity =>
        {
            entity.HasKey(e => e.Idtipofavorito).HasName("PRIMARY");

            entity.ToTable("tipofavorito");

            entity.Property(e => e.Idtipofavorito)
                .ValueGeneratedNever()
                .HasColumnName("idtipofavorito");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipopromocion>(entity =>
        {
            entity.HasKey(e => e.Idtipopromocion).HasName("PRIMARY");

            entity.ToTable("tipopromocion");

            entity.Property(e => e.Idtipopromocion)
                .ValueGeneratedNever()
                .HasColumnName("idtipopromocion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipotarifa>(entity =>
        {
            entity.HasKey(e => e.Idtipotarifa).HasName("PRIMARY");

            entity.ToTable("tipotarifa");

            entity.Property(e => e.Idtipotarifa)
                .ValueGeneratedNever()
                .HasColumnName("idtipotarifa");
            entity.Property(e => e.Activo)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipotransaccion>(entity =>
        {
            entity.HasKey(e => e.Idtipotransaccion).HasName("PRIMARY");

            entity.ToTable("tipotransaccion");

            entity.Property(e => e.Idtipotransaccion)
                .ValueGeneratedNever()
                .HasColumnName("idtipotransaccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipovariable>(entity =>
        {
            entity.HasKey(e => e.Idtipovariable).HasName("PRIMARY");

            entity.ToTable("tipovariable");

            entity.Property(e => e.Idtipovariable)
                .ValueGeneratedNever()
                .HasColumnName("idtipovariable");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipovehiculo>(entity =>
        {
            entity.HasKey(e => e.Idtipovehiculo).HasName("PRIMARY");

            entity.ToTable("tipovehiculo");

            entity.Property(e => e.Idtipovehiculo).HasColumnName("idtipovehiculo");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Asientos).HasColumnName("asientos");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(45)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => new { e.Idtoken, e.UsuarioIdusuario })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("token");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_token_usuario1_idx");

            entity.Property(e => e.Idtoken)
                .ValueGeneratedOnAdd()
                .HasColumnName("idtoken");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Token1)
                .HasMaxLength(2000)
                .HasColumnName("token");
            entity.Property(e => e.TokenRefresh)
                .HasMaxLength(45)
                .HasColumnName("token_refresh");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_token_usuario1");
        });

        modelBuilder.Entity<Tokenpago>(entity =>
        {
            entity.HasKey(e => new { e.Idtokenpago, e.UsuarioIdusuario })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("tokenpago");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_token_usuariopago_idx");

            entity.Property(e => e.Idtokenpago)
                .ValueGeneratedOnAdd()
                .HasColumnName("idtokenpago");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Token)
                .HasMaxLength(2000)
                .HasColumnName("token");
            entity.Property(e => e.TokenRefresh)
                .HasMaxLength(2000)
                .HasColumnName("token_refresh");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.Tokenpagos)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_token_usuariopago");
        });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.Idtransaccion).HasName("PRIMARY");

            entity.ToTable("transaccion");

            entity.HasIndex(e => e.EstatustransaccionIdestatustransaccion, "fk_transaccion_estatustransaccion1_idx");

            entity.HasIndex(e => e.MetodopagoIdmetodopago, "fk_transaccion_metodopago1_idx");

            entity.HasIndex(e => e.TipotransaccionIdtipotransaccion, "fk_transaccion_tipotransaccion1_idx");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_transaccion_usuario1_idx");

            entity.Property(e => e.Idtransaccion).HasColumnName("idtransaccion");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.Codigo)
                .HasMaxLength(40)
                .HasColumnName("codigo");
            entity.Property(e => e.Codigoexp)
                .HasColumnType("datetime")
                .HasColumnName("codigoexp");
            entity.Property(e => e.EstatustransaccionIdestatustransaccion).HasColumnName("estatustransaccion_idestatustransaccion");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.MetodopagoIdmetodopago).HasColumnName("metodopago_idmetodopago");
            entity.Property(e => e.Numeroordenpay)
                .HasMaxLength(36)
                .HasColumnName("numeroordenpay");
            entity.Property(e => e.Numeropeticionpay)
                .HasMaxLength(36)
                .HasColumnName("numeropeticionpay");
            entity.Property(e => e.Terminacion)
                .HasMaxLength(4)
                .HasColumnName("terminacion");
            entity.Property(e => e.TipotransaccionIdtipotransaccion).HasColumnName("tipotransaccion_idtipotransaccion");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");

            entity.HasOne(d => d.EstatustransaccionIdestatustransaccionNavigation).WithMany(p => p.Transaccions)
                .HasForeignKey(d => d.EstatustransaccionIdestatustransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transaccion_estatustransaccion1");

            entity.HasOne(d => d.MetodopagoIdmetodopagoNavigation).WithMany(p => p.Transaccions)
                .HasForeignKey(d => d.MetodopagoIdmetodopago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transaccion_metodopago1");

            entity.HasOne(d => d.TipotransaccionIdtipotransaccionNavigation).WithMany(p => p.Transaccions)
                .HasForeignKey(d => d.TipotransaccionIdtipotransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transaccion_tipotransaccion1");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.Transaccions)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transaccion_usuario1");
        });

        modelBuilder.Entity<Transaccionordenstripe>(entity =>
        {
            entity.HasKey(e => e.TransaccionOrdenStripeId).HasName("PRIMARY");

            entity
                .ToTable("transaccionordenstripe")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NumeroOrdern).HasMaxLength(36);
            entity.Property(e => e.NumeroPeticion).HasMaxLength(36);
            entity.Property(e => e.StripeId).HasMaxLength(150);
        });

        modelBuilder.Entity<Usoscfdireceptor>(entity =>
        {
            entity.HasKey(e => e.Idusoscfdireceptor).HasName("PRIMARY");

            entity.ToTable("usoscfdireceptor");

            entity.HasIndex(e => e.Clave, "clave_UNIQUE").IsUnique();

            entity.Property(e => e.Idusoscfdireceptor).HasColumnName("idusoscfdireceptor");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Clave)
                .HasMaxLength(10)
                .HasColumnName("clave");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Fisica)
                .HasColumnType("bit(1)")
                .HasColumnName("fisica");
            entity.Property(e => e.Moral)
                .HasColumnType("bit(1)")
                .HasColumnName("moral");
        });

        modelBuilder.Entity<UsoscfdireceptorRegimenfiscalreceptor>(entity =>
        {
            entity.HasKey(e => e.IdusoscfdireceptorRegimenfiscalreceptor).HasName("PRIMARY");

            entity.ToTable("usoscfdireceptor_regimenfiscalreceptor");

            entity.HasIndex(e => e.Idregimenfiscalreceptor, "fk_usoscfdireceptor_regimenfiscalreceptor_regimenfiscalrece_idx");

            entity.HasIndex(e => e.Idusoscfdireceptor, "fk_usoscfdireceptor_regimenfiscalreceptor_usoscfdireceptor_idx");

            entity.Property(e => e.IdusoscfdireceptorRegimenfiscalreceptor).HasColumnName("idusoscfdireceptor_regimenfiscalreceptor");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Fisica)
                .HasColumnType("bit(1)")
                .HasColumnName("fisica");
            entity.Property(e => e.Idregimenfiscalreceptor).HasColumnName("idregimenfiscalreceptor");
            entity.Property(e => e.Idusoscfdireceptor).HasColumnName("idusoscfdireceptor");
            entity.Property(e => e.Moral)
                .HasColumnType("bit(1)")
                .HasColumnName("moral");

            entity.HasOne(d => d.IdregimenfiscalreceptorNavigation).WithMany(p => p.UsoscfdireceptorRegimenfiscalreceptors)
                .HasForeignKey(d => d.Idregimenfiscalreceptor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usoscfdireceptor_regimenfiscalreceptor_regimenfiscalreceptor");

            entity.HasOne(d => d.IdusoscfdireceptorNavigation).WithMany(p => p.UsoscfdireceptorRegimenfiscalreceptors)
                .HasForeignKey(d => d.Idusoscfdireceptor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usoscfdireceptor_regimenfiscalreceptor_usoscfdireceptor");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Idusuario).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.CodigopaisIdcodigopais, "fk_usuario_codigopais1_idx");

            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.CodigoAuth)
                .HasMaxLength(45)
                .HasColumnName("codigo_auth");
            entity.Property(e => e.CodigoExp)
                .HasColumnType("datetime")
                .HasColumnName("codigo_exp");
            entity.Property(e => e.CodigopaisIdcodigopais).HasColumnName("codigopais_idcodigopais");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .HasColumnName("email");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.FirebaseId)
                .HasMaxLength(260)
                .HasColumnName("firebase_id");
            entity.Property(e => e.Hashpass)
                .HasMaxLength(150)
                .HasColumnName("hashpass");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(45)
                .HasColumnName("telefono");
            entity.Property(e => e.Validado)
                .HasColumnType("bit(1)")
                .HasColumnName("validado");

            entity.HasOne(d => d.CodigopaisIdcodigopaisNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.CodigopaisIdcodigopais)
                .HasConstraintName("fk_usuario_codigopais1");
        });

        modelBuilder.Entity<Variable>(entity =>
        {
            entity.HasKey(e => e.Idvariable).HasName("PRIMARY");

            entity.ToTable("variable");

            entity.HasIndex(e => e.TipovariableIdtipovariable, "fk_idtipovariable_idx");

            entity.Property(e => e.Idvariable).HasColumnName("idvariable");
            entity.Property(e => e.Codigo)
                .HasMaxLength(45)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .HasColumnName("descripcion");
            entity.Property(e => e.Lista).HasColumnName("lista");
            entity.Property(e => e.TipovariableIdtipovariable).HasColumnName("tipovariable_idtipovariable");

            entity.HasOne(d => d.TipovariableIdtipovariableNavigation).WithMany(p => p.Variables)
                .HasForeignKey(d => d.TipovariableIdtipovariable)
                .HasConstraintName("fk_idtipovariable");
        });

        modelBuilder.Entity<Variableempresa>(entity =>
        {
            entity.HasKey(e => new { e.VariableIdvariable, e.EmpresaIdempresa })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("variableempresa");

            entity.HasIndex(e => e.EmpresaIdempresa, "fk_empresa_idx");

            entity.Property(e => e.VariableIdvariable).HasColumnName("variable_idvariable");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.Valor)
                .HasMaxLength(250)
                .HasColumnName("valor");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.Variableempresas)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_empresa_variableempresa");

            entity.HasOne(d => d.VariableIdvariableNavigation).WithMany(p => p.Variableempresas)
                .HasForeignKey(d => d.VariableIdvariable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_variable_variableempresa");
        });

        modelBuilder.Entity<Variableglobal>(entity =>
        {
            entity.HasKey(e => e.VariableIdvariable).HasName("PRIMARY");

            entity.ToTable("variableglobal");

            entity.Property(e => e.VariableIdvariable)
                .ValueGeneratedNever()
                .HasColumnName("variable_idvariable");
            entity.Property(e => e.Valor)
                .HasMaxLength(250)
                .HasColumnName("valor");

            entity.HasOne(d => d.VariableIdvariableNavigation).WithOne(p => p.Variableglobal)
                .HasForeignKey<Variableglobal>(d => d.VariableIdvariable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_variable");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.Idvehiculo).HasName("PRIMARY");

            entity.ToTable("vehiculo");

            entity.HasIndex(e => e.EmpresaIdempresa, "fk_vehiculo_empresa1_idx");

            entity.HasIndex(e => e.TipovehiculoIdtipovehiculo, "fk_vehiculo_tipovehiculo1_idx");

            entity.Property(e => e.Idvehiculo).HasColumnName("idvehiculo");
            entity.Property(e => e.Activo)
                .HasColumnType("bit(1)")
                .HasColumnName("activo");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.Fechafinseguro)
                .HasColumnType("datetime")
                .HasColumnName("fechafinseguro");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(100)
                .HasColumnName("modelo");
            entity.Property(e => e.Numeroeconomico)
                .HasMaxLength(100)
                .HasColumnName("numeroeconomico");
            entity.Property(e => e.Numeromotor)
                .HasMaxLength(100)
                .HasColumnName("numeromotor");
            entity.Property(e => e.Numeropoliza)
                .HasMaxLength(20)
                .HasColumnName("numeropoliza");
            entity.Property(e => e.Placas)
                .HasMaxLength(8)
                .HasColumnName("placas");
            entity.Property(e => e.TipovehiculoIdtipovehiculo).HasColumnName("tipovehiculo_idtipovehiculo");
            entity.Property(e => e.Vin)
                .HasMaxLength(20)
                .HasColumnName("vin");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .HasConstraintName("fk_vehiculo_empresa1");

            entity.HasOne(d => d.TipovehiculoIdtipovehiculoNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.TipovehiculoIdtipovehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vehiculo_tipovehiculo1");
        });

        modelBuilder.Entity<Vehiculoservicio>(entity =>
        {
            entity.HasKey(e => e.Idvehiculoservicio).HasName("PRIMARY");

            entity.ToTable("vehiculoservicio");

            entity.HasIndex(e => e.Idvehiculo, "vehiculoservicio_vehiculo_FK");

            entity.Property(e => e.Idvehiculoservicio).HasColumnName("idvehiculoservicio");
            entity.Property(e => e.Detalle)
                .HasMaxLength(350)
                .HasColumnName("detalle");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Fechaservicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaservicio");
            entity.Property(e => e.Idvehiculo).HasColumnName("idvehiculo");

            entity.HasOne(d => d.IdvehiculoNavigation).WithMany(p => p.Vehiculoservicios)
                .HasForeignKey(d => d.Idvehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vehiculoservicio_vehiculo_FK");
        });

        modelBuilder.Entity<Viaje>(entity =>
        {
            entity.HasKey(e => e.Idviaje).HasName("PRIMARY");

            entity.ToTable("viaje");

            entity.HasIndex(e => e.CorridaAsignacionIdcorridaAsignacion, "fk_viaje_corridaasigancion_idx");

            entity.HasIndex(e => e.EmpresaIdempresa, "fk_viaje_empresa1_idx");

            entity.HasIndex(e => e.EstatusviajeIdestatusviaje, "fk_viaje_estatusviaje1_idx");

            entity.HasIndex(e => e.ParadaInicio, "fk_viaje_parada1_idx");

            entity.HasIndex(e => e.ParadaFin, "fk_viaje_parada2_idx");

            entity.HasIndex(e => e.PromocionIdpromocion, "fk_viaje_promocion_idx");

            entity.HasIndex(e => e.TransaccionIdtransaccion, "fk_viaje_transaccion1_idx");

            entity.HasIndex(e => e.UsuarioIdusuario, "fk_viaje_usuario1_idx");

            entity.HasIndex(e => e.ViajeredondoIdviajeredondo, "fk_viaje_viajeredondo_idx");

            entity.Property(e => e.Idviaje).HasColumnName("idviaje");
            entity.Property(e => e.Boleto)
                .HasMaxLength(10)
                .HasColumnName("boleto");
            entity.Property(e => e.CorridaAsignacionIdcorridaAsignacion).HasColumnName("corrida_asignacion_idcorrida_asignacion");
            entity.Property(e => e.CostoFinalTotal)
                .HasPrecision(10, 2)
                .HasColumnName("costo_final_total");
            entity.Property(e => e.CostoFinalUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("costo_final_unitario");
            entity.Property(e => e.CostoTarifaTotal)
                .HasPrecision(10, 2)
                .HasColumnName("costo_tarifa_total");
            entity.Property(e => e.CostoTarifaUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("costo_tarifa_unitario");
            entity.Property(e => e.DescuentoIddescuento).HasColumnName("descuento_iddescuento");
            entity.Property(e => e.EmpresaIdempresa).HasColumnName("empresa_idempresa");
            entity.Property(e => e.EstatusviajeIdestatusviaje).HasColumnName("estatusviaje_idestatusviaje");
            entity.Property(e => e.Facturado).HasColumnName("facturado");
            entity.Property(e => e.Fechacheckin)
                .HasColumnType("datetime")
                .HasColumnName("fechacheckin");
            entity.Property(e => e.Fechacheckout)
                .HasColumnType("datetime")
                .HasColumnName("fechacheckout");
            entity.Property(e => e.Fechacompra)
                .HasColumnType("datetime")
                .HasColumnName("fechacompra");
            entity.Property(e => e.Fechaviaje)
                .HasColumnType("datetime")
                .HasColumnName("fechaviaje");
            entity.Property(e => e.Numeropasajeros).HasColumnName("numeropasajeros");
            entity.Property(e => e.ParadaFin).HasColumnName("parada_fin");
            entity.Property(e => e.ParadaInicio).HasColumnName("parada_inicio");
            entity.Property(e => e.PromocionIdpromocion).HasColumnName("promocion_idpromocion");
            entity.Property(e => e.TransaccionIdtransaccion).HasColumnName("transaccion_idtransaccion");
            entity.Property(e => e.UsuarioIdusuario).HasColumnName("usuario_idusuario");
            entity.Property(e => e.ViajeredondoIdviajeredondo).HasColumnName("viajeredondo_idviajeredondo");
            entity.Property(e => e.Vigenciareserva)
                .HasColumnType("datetime")
                .HasColumnName("vigenciareserva");

            entity.HasOne(d => d.CorridaAsignacionIdcorridaAsignacionNavigation).WithMany(p => p.Viajes)
                .HasForeignKey(d => d.CorridaAsignacionIdcorridaAsignacion)
                .HasConstraintName("fk_viaje_corridaasigancion");

            entity.HasOne(d => d.EmpresaIdempresaNavigation).WithMany(p => p.Viajes)
                .HasForeignKey(d => d.EmpresaIdempresa)
                .HasConstraintName("fk_viaje_empresa1");

            entity.HasOne(d => d.EstatusviajeIdestatusviajeNavigation).WithMany(p => p.Viajes)
                .HasForeignKey(d => d.EstatusviajeIdestatusviaje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_viaje_estatusviaje1");

            entity.HasOne(d => d.ParadaFinNavigation).WithMany(p => p.ViajeParadaFinNavigations)
                .HasForeignKey(d => d.ParadaFin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_viaje_parada2");

            entity.HasOne(d => d.ParadaInicioNavigation).WithMany(p => p.ViajeParadaInicioNavigations)
                .HasForeignKey(d => d.ParadaInicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_viaje_parada1");

            entity.HasOne(d => d.PromocionIdpromocionNavigation).WithMany(p => p.Viajes)
                .HasForeignKey(d => d.PromocionIdpromocion)
                .HasConstraintName("fk_viaje_promocion");

            entity.HasOne(d => d.TransaccionIdtransaccionNavigation).WithMany(p => p.Viajes)
                .HasForeignKey(d => d.TransaccionIdtransaccion)
                .HasConstraintName("fk_viaje_transaccion1");

            entity.HasOne(d => d.UsuarioIdusuarioNavigation).WithMany(p => p.Viajes)
                .HasForeignKey(d => d.UsuarioIdusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_viaje_usuario1");

            entity.HasOne(d => d.ViajeredondoIdviajeredondoNavigation).WithMany(p => p.Viajes)
                .HasForeignKey(d => d.ViajeredondoIdviajeredondo)
                .HasConstraintName("fk_viaje_viajeredondo");
        });

        modelBuilder.Entity<Viajeredondo>(entity =>
        {
            entity.HasKey(e => e.Idviajeredondo).HasName("PRIMARY");

            entity.ToTable("viajeredondo");

            entity.HasIndex(e => e.Idviajeredondo, "fk_idviajeredondo_idx");

            entity.Property(e => e.Idviajeredondo).HasColumnName("idviajeredondo");
            entity.Property(e => e.Destinodireccion)
                .HasColumnType("mediumtext")
                .HasColumnName("destinodireccion");
            entity.Property(e => e.Destinolatitud)
                .HasPrecision(20, 8)
                .HasColumnName("destinolatitud");
            entity.Property(e => e.Destinolongitud)
                .HasPrecision(20, 8)
                .HasColumnName("destinolongitud");
            entity.Property(e => e.Destinonombre)
                .HasMaxLength(1000)
                .HasColumnName("destinonombre");
            entity.Property(e => e.Fecharegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Origendireccion)
                .HasColumnType("mediumtext")
                .HasColumnName("origendireccion");
            entity.Property(e => e.Origenlatitud)
                .HasPrecision(20, 8)
                .HasColumnName("origenlatitud");
            entity.Property(e => e.Origenlongitud)
                .HasPrecision(20, 8)
                .HasColumnName("origenlongitud");
            entity.Property(e => e.Origennombre)
                .HasMaxLength(1000)
                .HasColumnName("origennombre");
        });

        modelBuilder.Entity<Zona>(entity =>
        {
            entity.HasKey(e => e.IdZona).HasName("PRIMARY");

            entity.ToTable("zona");

            entity.Property(e => e.IdZona).HasColumnName("id_zona");
            entity.Property(e => e.NombreZona)
                .HasMaxLength(255)
                .HasColumnName("nombre_zona");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
