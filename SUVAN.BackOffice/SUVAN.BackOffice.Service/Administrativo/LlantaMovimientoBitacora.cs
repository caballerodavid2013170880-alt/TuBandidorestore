namespace SUVAN.BackOffice.Service.Administrativo
{
    public static class LlantaMovimientoBitacoraTipo
    {
        public const string Asignacion = "ASIGNACION";
        public const string Retiro = "RETIRO";
        public const string Reemplazo = "REEMPLAZO";
        public const string Rotacion = "ROTACION";
        public const string Correccion = "CORRECCION";

        private static readonly HashSet<string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
        {
            Asignacion,
            Retiro,
            Reemplazo,
            Rotacion,
            Correccion
        };

        public static bool EsValido(string? tipoMovimiento)
        {
            return !string.IsNullOrWhiteSpace(tipoMovimiento)
                && TiposPermitidos.Contains(tipoMovimiento.Trim());
        }

        public static string Normalizar(string tipoMovimiento)
        {
            return tipoMovimiento.Trim().ToUpperInvariant();
        }
    }

    public class LlantaAsignacionBitacoraMovimiento
    {
        public Guid? IdOperacion { get; set; }
        public ulong IdLlanta { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public ulong? IdLlantaAsignacionOrigen { get; set; }
        public ulong? IdVehiculoOrigen { get; set; }
        public ulong? IdVehiculoEjeOrigen { get; set; }
        public ushort? PosicionOrigen { get; set; }
        public ulong? IdLlantaAsignacionDestino { get; set; }
        public ulong? IdVehiculoDestino { get; set; }
        public ulong? IdVehiculoEjeDestino { get; set; }
        public ushort? PosicionDestino { get; set; }
        public uint? Kilometraje { get; set; }
        public string? Observaciones { get; set; }
        public uint CreadoPor { get; set; }
    }
}
