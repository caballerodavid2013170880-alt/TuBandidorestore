using SUVAN.BackOffice.Models.ViewModel.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Comercial
{
    public interface IPromocionesService
    {
        public Task<List<PromocionesViewModel>> ConsultaPromocionesAsync(int empresaId);
        public Task<PromocionesViewModel> ConsultaPromocionAsync(int? promocionId);
        public Task<PromocionesViewModel> GeneraPromocionAsync(PromocionesViewModel PromocionesViewModel);
        public Task<PromocionesViewModel> EliminaPromocionAsync(PromocionesViewModel model);
        public Task<List<TipoDescuentoViewModel>> TipoDescuentoAsync();
        public Task<List<TipoPromocionViewModel>> TipoPromocionAsync();
        public Task<UsuarioEmpresaModel> ConsultaEmpresaUsuarioAsync(int userId);
        public Task<List<RutaEmpresaViewModel>> ConsultaRutaCorridaAsync(int empresaId);
    }
}
