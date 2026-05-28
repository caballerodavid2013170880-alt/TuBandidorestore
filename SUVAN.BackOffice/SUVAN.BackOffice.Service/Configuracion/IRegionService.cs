using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Nuevo
namespace SUVAN.BackOffice.Service.Configuracion
{
    public interface IRegionService
    {
        /// <summary>
        /// Obtiene el listado de regiones de la empresa indicada,
        /// incluyendo la navegación a <see cref="Empresa"/> (propiedad <c>IdEmpresaNavigation</c>)
        /// para poder mostrar el nombre de la empresa en la tabla de la vista.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// Se utiliza como filtro de seguridad para restringir los resultados.
        /// </param>
        /// <returns>
        /// Lista de entidades <see cref="Region"/> con la propiedad de navegación
        /// <c>IdEmpresaNavigation</c> cargada mediante eager loading.
        /// </returns>
        Task<List<Region>> GetRegiones(int idEmpresa);
        /// <summary>
        /// Genera y devuelve el <see cref="RegionViewModel"/> necesario para
        /// renderizar el formulario agregar/editar de una región.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.Asigna al campo IdEmpresa del ViewModel.
        /// </param>
        /// <param name="idRegion">
        /// Identificador de la región a editar. Pasa 0 para agregar.
        /// </param>
        /// <returns>
        /// <see cref="RegionViewModel"/> contiene con los datos de la región (si existe) y vacío para una nueva región.
        /// </returns>
        /// <exception cref="Exception">
        /// Si <paramref name="idRegion"/> es mayor a 0 y la región no existe o no pertenece a la empresa del usuario.
        /// </exception>
        Task<RegionViewModel> GetRegionViewModel(int idEmpresa, int idRegion);
        /// <summary>
        /// Agrega o actualiza una región en la base de datos.
        /// Aplica las siguientes validaciones de seguridad y negocio:
        /// <list type="bullet">
        ///   <item>La empresa del ViewModel coincide con la empresa del usuario autenticado.</item>
        ///   <item>Al Editar, la región existe y pertenece a la empresa del usuario.</item>
        ///   <item>No existe una región con el mismo nombre dentro de la misma empresa.</item>
        /// </list>
        /// </summary>
        /// <param name="model">ViewModel con los datos capturados en el formulario.</param>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// Se usa para sobrescribir el campo empresa en la entidad y validar seguridad.
        /// </param>
        /// <returns><c>true</c> si la operación fue exitosa.</returns>
        /// <exception cref="Exception">
        /// Si alguna validación de seguridad o de negocio falla.
        /// </exception>
        Task<bool> AgregarRegion(RegionViewModel model, int idEmpresa);
    }
}
