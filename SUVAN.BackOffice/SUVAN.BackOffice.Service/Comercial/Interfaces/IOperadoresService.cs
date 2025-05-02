using SUVAN.BackOffice.Models.ViewModel.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Comercial
{
    public interface IOperadoresService
    {
        public Task<List<ObtenerOperadoresViewModel>> ObtenerOperadores(int empresaId);
        public Task<ObtenerDetalleOperadorViewModel> ObtenerDetalleOperador(int operadorId);
        public Task<List<ObtenerViajesOperadorViewModel>> ObtenerViajesOperador(int operadorId);
        public Task<List<ObtenerViajesOperadorViewModel>> ObtenerViajesProximosOperador(int operadorId);
        public Task<List<CalificacionOperadorViewModel>> CalificacionOperador(int operadorId);
        public Task<PagosOperadorViewModel> PagosOperador(int operadorId);
        public Task<List<AsignacionesOperadorViewModel>> AsignacionesOperador(int operadorId);
    }
}
