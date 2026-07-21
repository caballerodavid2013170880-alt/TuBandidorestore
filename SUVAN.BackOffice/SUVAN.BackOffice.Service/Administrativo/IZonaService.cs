using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface IZonaService
    {
       //Remake Cat Zona
        /// <summary>
        /// Obtiene el listado de Zonas de la empresa indicada, incluyendo navegación a Región y Planta.
        /// </summary>
        Task<List<Zona>> GetZona(int idEmpresa);

        /// <summary>
        /// Construye el ViewModel para el formulario de alta o edición de una zona.
        /// </summary>
        Task<ZonaViewModel> GetZonaViewModel(int idEmpresa, int idZona);

        /// <summary>
        /// Agrega o actualiza una zona, validando jerarquía Región -> Planta.
        /// </summary>
        Task<bool> AgregarZona(ZonaViewModel model, int idEmpresa);

        /// <summary>
        /// Elimina lógicamente o físicamente una zona.
        /// </summary>
        Task<bool> EliminarZona(int IdZona, int idEmpresa);
    }
}
