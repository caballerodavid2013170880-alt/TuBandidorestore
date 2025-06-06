using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IMantenimientoService
    {
        Task<List<TallerViewModel>> ObtenerTaller(int IdEmpresa);

        Task<List<MecanicoViewModel>> ObtenerMecanico(int IdEmpresa);
    }
}
