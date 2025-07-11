using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class VehiculoDetalleViewModel
    {
        public int IdVehiculoDetalle { get; set; }
        public string MarcasJson { get; set; } = string.Empty;
        public string ZonaJson { get; set; } = string.Empty;

        // Relaciones con otras entidades
        public int IdVehiculo { get; set; }
        public string? PlacasVehiculo { get; set; } = null!;
        public string PlacasIdVehiculo => $"{IdVehiculo} - {PlacasVehiculo}";
        public sbyte IdTipoVehiculo { get; set; }
        public string? NombreTipoV { get; set; } = null!;
        public short IdMarca { get; set; }
        public string? DescripcionMarca { get; set; }
        public int IdZona { get; set; }
        public string? NombreZona { get; set; }
        public int IdDeposito { get; set; }
        public string? NombreDeposito { get; set; }
        public int? IdEspecificacion { get; set; }
        public int IdModelo { get; set; }
        public string? DescripcionModelo { get; set; }
        public int IdTipoEje { get; set; }
        public string? DescripcionEje { get; set; }
        public int? IdNegocio { get; set; }

        // Información general del vehículo
        public int AnioVehiculo { get; set; }
        public string ColorVehiculo { get; set; } = null!;
        public ulong? TieneRotulo { get; set; }
        public string NumeroSerie { get; set; } = null!;
        public string NumeroMotor { get; set; } = null!;
        public string Carroceria { get; set; } = null!;
        public string TarjetaCirculacion { get; set; } = null!;

        // Documentación y permisos
        public int Gasolina { get; set; }
        public ulong? CopiaFactura { get; set; }
        public ulong? CopiaTarjetaCir { get; set; }
        public ulong? CopiaPlaca { get; set; }
        public ulong? CopiaVerificacion { get; set; }
        public ulong? CopiaPolizaSeguro { get; set; }
        public string Proveedor { get; set; } = null!;
        public DateTime? FechaCompra { get; set; }
        public string NumeroFactura { get; set; } = null!;
        public float CostoVehiculo { get; set; }
        public ulong? TarifaVehicular { get; set; }
        public string? NombreTarifaVehicular { get; set; }
        public float KilometrajeAcumulado { get; set; }

        // Estado del vehículo
        public DateTime? FechaBaja { get; set; }
        public string ColorInterior { get; set; } = null!;
        public ulong? TieneCaja { get; set; }
        public ulong? NecesitaRemolque { get; set; }
        public ulong? VehiculoRelevo { get; set; }
        public ulong? Rentado { get; set; }

        // Registro y recuperación
        public float KilometrajeGarantia { get; set; }
        public int MesesGarantia { get; set; }
        public string? EconomicoAnterior { get; set; }

        // Especificaciones de carga
        public int PesoMinimo { get; set; }
        public int PesoMaximo { get; set; }
        public float VolumenMinimo { get; set; }
        public float VolumenMaximo { get; set; }

        // Licencia y permisos
        public string TipoLicenciaRequerida { get; set; } = null!;
        public ulong? PermisoCargaAceite { get; set; }
        public DateTime VigenciaPermisoAceite { get; set; }
        public DateTime? VigenciaTarjetaCirculacion { get; set; }
        public string? UsuarioCaptura { get; set; }

        public List<MarcaViewModel> Marcas { get; set; } = new();

        public List<ModeloViewModel> Modelos { get; set; } = new();

        public List<TipoVehiculoViewModel> TipoVehiculo { get; set; } = new();

        public List<BajaVehiViewModel> BajaVehiculo { get; set; } = new();

        public List<CausaBajaViewModel> CausaBaja { get; set; } = new();

        public List<ZonaViewModel> Zonas { get; set; } = new();

        public List<VehiViewModel> Vehiculos { get; set; } = new();

        public class VehiViewModel {

            public int IdVehiculo { get; set; }

            public string? Placas { get; set; }

            public string? Vin { get; set; }

            public string? Numeroeconomico { get; set; }

            public string? Numeromotor { get; set; }
        }

        public class ModeloViewModel
        {
            public short IdMarca { get; set; }

            public int IdModelo { get; set; }

            public sbyte IdTipoV { get; set; }

            public int AnioDesde { get; set; }

            public int AnioHasta { get; set; }

            public string Descripcion { get; set; } = null!;

            public float KmGarantia { get; set; }

            public int MesGarantia { get; set; }

            public int? TipoEje { get; set; }

            public string DescripcionModeloId => $"{IdModelo} - {Descripcion}";

            public List<MarcaViewModel> MarcasView { get; set; } = new();

            public List<TipoVehiculoViewModel> TipoVehiculoView { get; set; } = new();
        }

        public class MarcaViewModel
        {
            public int IdMarca { get; set; }

            public string Descripcion { get; set; } = null!;

            public string DescripcionMarcaId => $"{IdMarca} - {Descripcion}";

            public List<ModeloViewModel> Modelos { get; set; } = new();
        }

        public class TipoVehiculoViewModel
        {
            public int TipoUnidadId { get; set; }

            public string? Nombre { get; set; } = null!;

            public string NombreTipoId => $"{TipoUnidadId} - {Nombre}";

        }

        public partial class CausaBajaViewModel
        {
            public int IdCausaBaja { get; set; }

            public int IdBaja { get; set; }

            public string Descripcion { get; set; } = null!;

            public List<BajaVehiViewModel> BajaVehiculoView { get; set; } = new();

        }

        public partial class BajaVehiViewModel
        {
            public int IdBaja { get; set; }
            public string Descripcion { get; set; } = null!;

            public string DescripcionId => $"{IdBaja} - {Descripcion}";

        }

    }
}