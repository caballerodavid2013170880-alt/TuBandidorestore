using SUVAN.BackOffice.Models.ViewModel.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Comercial
{
    public interface IUnidadesService
    {
        public Task<List<ObtenerUnidadesViewModel>> ObtenerUnidades(int empresaId);
        public Task<ObtenerUnidadesViewModel> ObtenerDetalleUnidad(int unidadId);
        public Task<List<ObtenerGeneralesUnidadesViewModel>> GeneralesUnidad(int unidadId);
        public Task<List<AsignacionesUnidadViewModel>> AsignacionesUnidad(int unidadId);
        public Task<List<SeguroUnidadViewModel>> SeguroUnidad(int unidadId);
    }
}
