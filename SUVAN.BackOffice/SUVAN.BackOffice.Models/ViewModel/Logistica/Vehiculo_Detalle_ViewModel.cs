using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class VehiculoDetalleViewModel
    {
        [Key]
        public int IdVehicDetalle { get; set; }

        // Relaciones con otras entidades
        public int IdZona { get; set; }
        public int IdDeposito { get; set; }
        public int IdMarca { get; set; }
        public int IdEspeci { get; set; }
        public int IdModelo { get; set; }
        public int IdPermisoAceite { get; set; }
        public int IdCognos { get; set; }
        public int IdTipoVehi { get; set; }

        // Información general del vehículo
        [Required]
        public string Negocio { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Rotulo { get; set; } = string.Empty;
        public string PlacaPE { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public string Motor { get; set; } = string.Empty;
        public string Carroc { get; set; } = string.Empty;
        public string TarCircula { get; set; } = string.Empty;

        // Documentación y permisos
        public string Gasoline { get; set; } = string.Empty;
        public bool Encierro { get; set; }
        public bool CopFac { get; set; }
        public bool CopTCir { get; set; }
        public bool CopPla { get; set; }
        public bool CopVer { get; set; }
        public bool CopPol { get; set; }
        public int NoCirc { get; set; }
        public string DnoCirc { get; set; } = string.Empty;
        public string Proveed { get; set; } = string.Empty;
        public DateTime? FCompra { get; set; }
        public string Factura { get; set; } = string.Empty;
        public decimal Costo { get; set; }
        public decimal TariAve { get; set; }
        public string NTariAve { get; set; } = string.Empty;
        public int KmAcum { get; set; }

        // Estado del vehículo
        public string StVehic { get; set; } = string.Empty;
        public DateTime? FBaja { get; set; }
        public string TipoEje { get; set; } = string.Empty;
        public string ColInt { get; set; } = string.Empty;
        public string RegFed { get; set; } = string.Empty;
        public string EdregPl { get; set; } = string.Empty;
        public string ColEst { get; set; } = string.Empty;
        public bool Caja { get; set; }
        public bool NecRem { get; set; }
        public bool Relevo { get; set; }
        public bool Rentado { get; set; }

        // Registro y recuperación
        public string ColRuta { get; set; } = string.Empty;
        public string CauBaja { get; set; } = string.Empty;
        public bool VRecuperado { get; set; }
        public int KmGaran { get; set; }
        public int MesGara { get; set; }
        public string EcoAnt { get; set; } = string.Empty;
        public string LocFor { get; set; } = string.Empty;
        public bool Baja { get; set; }

        // Especificaciones de carga
        public decimal PesoMinimo { get; set; }
        public decimal PesoMaximo { get; set; }
        public decimal VolumenMinimo { get; set; }
        public decimal VolumenMaximo { get; set; }

        // Licencia y permisos
        public string TipoLicenciaRequerida { get; set; } = string.Empty;
        public bool PermisoCargaAceite { get; set; }
        public int IdVehiculo { get; set; }
        public DateTime? VigenciaPermisoAceite { get; set; }
        public DateTime? VigenciaTarjetaCircula { get; set; }
    }
}