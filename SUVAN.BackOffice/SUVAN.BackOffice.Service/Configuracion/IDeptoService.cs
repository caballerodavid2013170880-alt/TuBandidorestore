using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
    /// <summary>
    /// Interfaz del servicio para la gestión de Departamentos.
    /// </summary>
    public interface IDeptoService
    {
        /// <summary>
        /// Agrega o actualiza un departamento en la base de datos simulando relaciones por código.
        /// </summary>
        /// <param name="model">ViewModel con los datos del departamento.</param>
        /// <param name="id_empresa">Identificador de la empresa actual.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario lanza excepción.</returns>
        Task<bool> AgregarDepto(DeptoViewModel model, int id_empresa);

        /// <summary>
        /// Obtiene el listado de Departamentos disponibles para la empresa del usuario.
        /// </summary>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <returns>Lista de Departamentos vinculados indirectamente a la empresa.</returns>
        Task<List<Depto>> GetDeptos(int id_empresa);

        /// <summary>
        /// Obtiene el ViewModel para un departamento específico para editarlo.
        /// </summary>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <param name="id_region">Identificador de la región.</param>
        /// <param name="id_planta">Identificador de la planta.</param>
        /// <param name="id_zona">Identificador de la zona.</param>
        /// <param name="id_deposi">Identificador del depósito.</param>
        /// <param name="id_depto">Identificador del departamento.</param>
        /// <returns>Instancia de DeptoViewModel con los datos o inicializada si es nuevo.</returns>
        Task<DeptoViewModel> GetDeptoViewModel(int id_empresa, short id_region, short id_planta, short id_zona, short id_deposi, short id_depto);

        /// <summary>
        /// Obtiene el listado de las zonas que pertenecen a las empresas vinculadas al usuario.
        /// </summary>
        /// /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <returns>Lista de zonas por empresa.</returns>

        Task<List<Zona>> GetZonas(int id_empresa);

        /// <summary>
        /// Obtiene el listado de las depositos que pertenecen a las empresas vinculadas al usuario.
        /// </summary>
        /// /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <returns>Lista de depositos por empresa y región.</returns>
        Task<List<Deposito>> GetDepositos(int id_empresa);
    }
}
