using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.MantenimientoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IMantenimientoService
    {
        Task<List<TallerViewModel>> ObtenerTaller(int IdEmpresa);

        Task<List<MecanicoViewModel>> ObtenerMecanico(int tallerId);

        Task<List<TipoReparacionViewModel>> ObtenerTipoReparacion();

        Task<List<CausaMantenimientoViewModel>> ObtenerCausaMantenimiento();
    }
}
