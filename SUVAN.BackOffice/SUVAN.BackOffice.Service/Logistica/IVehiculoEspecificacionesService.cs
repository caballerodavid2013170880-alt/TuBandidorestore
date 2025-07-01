using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoEspecificacionesViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IVehiculoEspecificacionesService
    {
        Task<List<MarcaEspecifiViewModel>> ObtenerMarcas();

        Task<List<ModeloEspecifiViewModel>> ObtenerModelo(int marcaId);

        Task<VehiculoEspecificacionesViewModel> GetVehiculoEspecifi();

        Task<VehiculoEspecificacionesViewModel> ObtenerEspecificaciones(VehiculoEspecificacionesViewModel model, int idModelo);
    }
}
