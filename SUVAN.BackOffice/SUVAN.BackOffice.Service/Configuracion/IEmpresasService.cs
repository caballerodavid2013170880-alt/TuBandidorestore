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
    public interface IEmpresasService
    {

        ///  /// <summary>
        /// Agrega o actualiza una empresa en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la empresa.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarEmpresa(AgregarEmpresaViewModel model);

        /// <summary>
        /// Obtiene el listado de empresas desde la base de datos.
        /// </summary>
        /// <returns>Lista de empresas.</returns>
        Task<List<Empresa>> GetEmpresas();
        List<TipoRegimenFiscalModel> ObtenerTipoRegimen();
        /// <summary>
        /// Obtiene el ViewModel para la empresa específica.
        /// </summary>
        /// <param name="id">Identificador de la empresa.</param>
        /// <returns>ViewModel para la empresa específica.</returns>
        /// 
        Task<AgregarEmpresaViewModel> GetEmpresasViewModel(int id);
    }
}
