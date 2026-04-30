using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
    /// <summary>
    /// Interfaz para el servicio de Departamentos (Depto).
    /// Proporciona los métodos para consultar y gestionar el catálogo de departamentos.
    /// </summary>
    public interface IDeptoService
    {
        /// <summary>
        /// Obtiene la lista de departamentos filtrada por la jerarquía geografica.
        /// </summary>
        Task<List<Depto>> GetDeptos(short id_region, short id_planta, short id_zona, short id_deposi);

        /// <summary>
        /// Obtiene un ViewModel específico de un departamento.
        /// </summary>
        Task<DeptoViewModel> GetDeptoViewModel(short id_region, short id_planta, short id_zona, short id_deposi, short id_depto);

        /// <summary>
        /// Agrega o actualiza un departamento validando sus referencias.
        /// </summary>
        Task<bool> AgregarDepto(DeptoViewModel model);
    }
}
