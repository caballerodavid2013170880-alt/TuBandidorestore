namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class LlantaViewModel
    {
        public ulong IdLlanta { get; set; }
        public string CodigoLlanta { get; set; } = string.Empty;
        public string NumeroSerieDot { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Estado { get; set; }
        public string? Deposito { get; set; }
        public DateOnly? FechaAdquisicion { get; set; }
        public decimal? CostoAdquisicion { get; set; }
    }
}
