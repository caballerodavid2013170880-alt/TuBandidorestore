namespace SUVAN.BackOffice.Models.ViewModel.Administrativo
{
    public class LlantaAsignacionViewModel
    {
        public int IdVehiculo { get; set; }
        public List<LlantaCrearViewModel.CatalogItemViewModel> Vehiculos { get; set; } = new();
    }
}
