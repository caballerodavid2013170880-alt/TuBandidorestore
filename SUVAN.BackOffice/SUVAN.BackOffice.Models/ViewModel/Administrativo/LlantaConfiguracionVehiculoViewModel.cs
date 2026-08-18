namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class LlantaConfiguracionVehiculoViewModel
    {
        public int IdVehiculo { get; set; }
        public string? NumeroEconomico { get; set; }
        public string? Placas { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public float? KilometrajeActual { get; set; }
        public List<LlantaConfiguracionEjeViewModel> Ejes { get; set; } = new();
    }

    public class LlantaConfiguracionEjeViewModel
    {
        public int IdVehiculoEje { get; set; }
        public ushort NumeroEje { get; set; }
        public int IdTipoEje { get; set; }
        public string NombreTipoEje { get; set; } = string.Empty;
        public string DescripcionTipoEje { get; set; } = string.Empty;
        public byte NumeroPosiciones { get; set; }
        public List<LlantaConfiguracionPosicionViewModel> Posiciones { get; set; } = new();
    }

    public class LlantaConfiguracionPosicionViewModel
    {
        public ushort NumeroPosicion { get; set; }
        public bool Ocupada { get; set; }
        public LlantaConfiguracionAsignacionViewModel? Asignacion { get; set; }
    }

    public class LlantaConfiguracionAsignacionViewModel
    {
        public ulong IdLlantaAsignacion { get; set; }
        public ulong IdLlanta { get; set; }
        public string CodigoLlanta { get; set; } = string.Empty;
        public string NumeroSerieDot { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public ushort IdEstadoLlanta { get; set; }
        public string EstadoLlanta { get; set; } = string.Empty;
        public DateTime FechaAsignacion { get; set; }
        public uint KmVehiculoAsignacion { get; set; }
    }
}
