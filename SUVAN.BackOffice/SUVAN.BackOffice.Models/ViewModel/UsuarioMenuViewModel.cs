using System.Collections.Generic;
namespace SUVAN.BackOffice.Models.ViewModel
{
    public class UsuarioEmpresaMenuViewModel
    {
        public int EmpresaId { get; set; }
        public string EmpresaNombre { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
        public List<UsuarioDepositoMenuViewModel> Depositos { get; set; } = new List<UsuarioDepositoMenuViewModel>();
    }
    public class UsuarioDepositoMenuViewModel
    {
        public int IdUsuarioJerarquia { get; set; }
        public int? DepositoId { get; set; }
        public string DepositoNombre { get; set; } = string.Empty;
        public string ZonaNombre { get; set; } = string.Empty;
        public string PlantaNombre { get; set; } = string.Empty;
        public string RegionNombre { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
    }
}