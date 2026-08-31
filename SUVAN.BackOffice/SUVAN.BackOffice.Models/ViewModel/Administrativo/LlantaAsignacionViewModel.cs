namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class LlantaAsignacionViewModel
    {
        public int IdVehiculo { get; set; }
        public List<LlantaCrearViewModel.CatalogItemViewModel> Vehiculos { get; set; } = new();
    }

    public class LlantaInstalacionViewModel
    {
        public int IdVehiculo { get; set; }
        public int IdVehiculoEje { get; set; }
        public ushort NumeroPosicion { get; set; }
        public ulong IdLlanta { get; set; }
        public ushort IdTipoAsignacion { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public uint KmVehiculoAsignacion { get; set; }
        public string? ObservacionesAsignacion { get; set; }
    }

    public class LlantaRetiroViewModel
    {
        public ulong IdLlantaAsignacion { get; set; }
        public DateTime FechaRetiro { get; set; }
        public uint KmVehiculoRetiro { get; set; }
        public ushort IdMotivoRetiro { get; set; }
        public ushort IdEstadoDestino { get; set; }
        public string? ObservacionesRetiro { get; set; }
    }

    public class LlantaReemplazoViewModel
    {
        public ulong IdLlantaAsignacion { get; set; }
        public ulong IdLlantaEntrante { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public uint KmVehiculo { get; set; }
        public ushort IdMotivoRetiro { get; set; }
        public ushort IdEstadoDestinoSaliente { get; set; }
        public string? ObservacionesRetiro { get; set; }
        public string? ObservacionesAsignacion { get; set; }
    }

    public class LlantaRotacionViewModel
    {
        public ulong IdLlantaAsignacionOrigen { get; set; }
        public int IdVehiculoEjeDestino { get; set; }
        public ushort NumeroPosicionDestino { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public uint KmVehiculo { get; set; }
        public string? Observaciones { get; set; }
    }
}
