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
    public interface IDeptoService
    {
        /// <summary>
        /// Obtiene el listado de departamentos de la empresa indicada,
        /// incluyendo la navegación a <see cref="Deposito"/>
        /// (<c>IdDepositoNavigation</c>) para mostrar el nombre del depósito en la tabla.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// Se utiliza como filtro de seguridad para restringir los resultados.
        /// </param>
        /// <returns>
        /// Lista de entidades <see cref="Depto"/> con la propiedad de navegación
        /// <c>IdDepositoNavigation</c> cargada mediante eager loading,
        /// ordenadas por nombre de depósito y luego por nombre de departamento.
        /// </returns>
        Task<List<Depto>> GetDepto(int idEmpresa);
        /// <summary>
        /// Construye y devuelve el <see cref="DeptoViewModel"/> necesario para
        /// renderizar el formulario de alta o edición de un departamento.
        /// <list type="bullet">
        ///   <item>
        ///     Al Agregar (<paramref name="idDepto"/> == 0) únicamente carga
        ///     la lista de Regiones; el resto de selectores se cargan en cascada vía AJAX.
        ///   </item>
        ///   <item>
        ///     Al Editar pre-carga los cuatro selectores con los datos del
        ///     departamento existente respetando la jerarquía de seguridad.
        ///   </item>
        /// </list>
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// Determina qué regiones, plantas, zonas y depósitos se muestran en los selectores.
        /// </param>
        /// <param name="idDepto">
        /// Identificador del departamento a editar.
        /// Pasar <c>0</c> para obtener un ViewModel vacío (modo alta).
        /// </param>
        /// <returns>
        /// <see cref="DeptoViewModel"/> poblado con regiones disponibles y, en modo edición,
        /// también con plantas, zonas, depósitos y datos del departamento existente.
        /// </returns>
        /// <exception cref="Exception">
        /// Si <paramref name="idDepto"/> es mayor a 0 y el departamento no existe
        /// o no pertenece a la empresa del usuario.
        /// </exception>
        Task<DeptoViewModel> GetDeptoViewModel(int idEmpresa, int idDepto);
        /// <summary>
        /// Inserta o actualiza un departamento en la base de datos.
        /// Aplica las siguientes validaciones de seguridad y negocio en orden jerárquico:
        /// <list type="bullet">
        ///   <item>La región seleccionada pertenece a la empresa del usuario.</item>
        ///   <item>La planta seleccionada pertenece a la región y empresa.</item>
        ///   <item>La zona seleccionada pertenece a la región, planta y empresa.</item>
        ///   <item>El depósito seleccionado pertenece a la región, planta, zona y empresa.</item>
        ///   <item>Al Editar, el departamento existe y pertenece a la empresa del usuario.</item>
        ///   <item>No existe un departamento con el mismo nombre en el mismo depósito y empresa.</item>
        /// </list>
        /// </summary>
        /// <param name="model">ViewModel con los datos capturados en el formulario.</param>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// Se utiliza para validar la jerarquía y sobrescribir el campo empresa en la entidad.
        /// </param>
        /// <returns><c>true</c> si la operación fue exitosa.</returns>
        /// <exception cref="Exception">
        /// Si alguna validación de seguridad o de negocio falla.
        /// </exception>
        Task<bool> AgregarDepto(DeptoViewModel model, int idEmpresa);
        /// <summary>
        /// Obtiene la lista de plantas disponibles para una región específica,
        /// filtradas por empresa del usuario. Se invoca desde el controlador como
        /// endpoint AJAX para la carga en cascada Región → Planta.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa del usuario autenticado.</param>
        /// <param name="idRegion">Identificador de la región seleccionada.</param>
        /// <returns>
        /// Lista de <see cref="DeptoViewModel.PlantaItemViewModel"/> ordenada por nombre.
        /// Lista vacía si la región no pertenece a la empresa.
        /// </returns>
        Task<List<DeptoViewModel.PlantaItemViewModel>> GetPlantasPorRegion(int idEmpresa, int idRegion);
        /// <summary>
        /// Obtiene la lista de zonas disponibles para una región y planta específicas,
        /// filtradas por empresa del usuario. Se invoca desde el controlador como
        /// endpoint AJAX para la carga en cascada Planta → Zona.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa del usuario autenticado.</param>
        /// <param name="idRegion">Identificador de la región seleccionada.</param>
        /// <param name="idPlanta">Identificador de la planta seleccionada.</param>
        /// <returns>
        /// Lista de <see cref="DeptoViewModel.ZonaItemViewModel"/> ordenada por nombre.
        /// Lista vacía si la combinación región-planta no pertenece a la empresa.
        /// </returns>
        Task<List<DeptoViewModel.ZonaItemViewModel>> GetZonasPorPlanta(int idEmpresa, int idRegion, int idPlanta);
        /// <summary>
        /// Obtiene la lista de depósitos disponibles para una región, planta y zona específicas,
        /// filtrados por empresa del usuario. Se invoca desde el controlador como
        /// endpoint AJAX para la carga en cascada Zona → Depósito.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa del usuario autenticado.</param>
        /// <param name="idRegion">Identificador de la región seleccionada.</param>
        /// <param name="idPlanta">Identificador de la planta seleccionada.</param>
        /// <param name="idZona">Identificador de la zona seleccionada.</param>
        /// <returns>
        /// Lista de <see cref="DeptoViewModel.DepositoItemViewModel"/> ordenada por nombre.
        /// Lista vacía si la combinación región-planta-zona no pertenece a la empresa.
        /// </returns>
        Task<List<DeptoViewModel.DepositoItemViewModel>> GetDepositosPorZona(int idEmpresa, int idRegion, int idPlanta, int idZona);
    }
}
