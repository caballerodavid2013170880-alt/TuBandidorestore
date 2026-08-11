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
}
