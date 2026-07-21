using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface IDeptoService
    {
        /// <summary>
        /// Obtiene el listado de departamentos de la empresa indicada,
        /// incluyendo la navegaci�n a <see cref="Deposito"/> (<c>IdDepositoNavigation</c>) para mostrar el nombre del dep�sito en la tabla.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.Se utiliza como filtro de seguridad para restringir los resultados.
        /// </param>
        /// <returns>
        /// Lista de entidades <see cref="Depto"/> con la propiedad de navegaci�n <c>IdDepositoNavigation</c> cargada mediante eager loading,
        Task<List<Depto>> GetDepto(int idEmpresa);

        /// <summary>
        /// Construye y devuelve el <see cref="DeptoViewModel"/> necesario para renderizar el formulario al agregar o editar de un departamento.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// </param>
        /// <param name="idDepto">
        /// Identificador del departamento a editar.Pasar <c>0</c> para obtener un ViewModel vac�o (al agregar).
        /// </param>
        /// <returns>
        /// <see cref="DeptoViewModel"/> poblado con regiones disponibles y, al editar, tambi�n con plantas, zonas, dep�sitos y datos del departamento existente.
        /// </returns>
        /// <exception cref="Exception">
        /// Si <paramref name="idDepto"/> es mayor a 0 y el departamento no existe o no pertenece a la empresa del usuario.
        /// </exception>
        Task<DeptoViewModel> GetDeptoViewModel(int idEmpresa, int idDepto);

        /// <summary>
        /// Inserta o actualiza un departamento en la base de datos.
        /// Aplica las siguientes validaciones de seguridad y negocio en orden jer�rquico:
        /// </summary>
        /// <param name="model">ViewModel con los datos capturados en el formulario.</param>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado. Se utiliza para validar la jerarqu�a y sobrescribir el campo empresa en la entidad.
        /// </param>
        /// <returns><c>true</c> si la operaci�n fue exitosa.</returns>
        Task<bool> AgregarDepto(DeptoViewModel model, int idEmpresa);
        /// <summary>
        /// Obtiene la lista de plantas disponibles para una regi�n espec�fica,
        /// filtradas por empresa del usuario. Invado desde el controlador como endpoint AJAX para la carga en cascada Regi�n ? Planta.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa del usuario autenticado.</param>
        /// <param name="idRegion">Identificador de la regi�n seleccionada.</param>
        /// <returns>
        /// Lista de <see cref="DeptoViewModel.CatalogItemViewModel"/> ordenada por nombre.
        /// </returns>
        Task<List<DeptoViewModel.CatalogItemViewModel>> GetPlantasPorRegion(int idEmpresa, int idRegion);
        /// <summary>
        /// Obtiene la lista de zonas disponibles para una regi�n y planta espec�ficas, filtradas por empresa del usuario. Se invoca desde el controlador como
        /// endpoint AJAX para la carga en cascada Planta ? Zona.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa del usuario autenticado.</param>
        /// <param name="idRegion">Identificador de la regi�n seleccionada.</param>
        /// <param name="idPlanta">Identificador de la planta seleccionada.</param>
        /// <returns>
        /// Lista de <see cref="DeptoViewModel.CatalogItemViewModel"/> ordenada por nombre.
        /// </returns>
        Task<List<DeptoViewModel.CatalogItemViewModel>> GetZonasPorPlanta(int idEmpresa, int idRegion, int idPlanta);
        /// <summary>
        /// Obtiene la lista de dep�sitos disponibles para una regi�n, planta y zona espec�ficas, filtrados por empresa del usuario. Se invoca desde el controlador como
        /// endpoint AJAX para la carga en cascada Zona ? Dep�sito.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa del usuario autenticado.</param>
        /// <param name="idRegion">Identificador de la regi�n seleccionada.</param>
        /// <param name="idPlanta">Identificador de la planta seleccionada.</param>
        /// <param name="idZona">Identificador de la zona seleccionada.</param>
        /// <returns>
        /// Lista de <see cref="DeptoViewModel.CatalogItemViewModel"/> ordenada por nombre.
        /// Lista vac�a si la combinaci�n regi�n-planta-zona no pertenece a la empresa.
        /// </returns>
        Task<List<DeptoViewModel.CatalogItemViewModel>> GetDepositosPorZona(int idEmpresa, int idRegion, int idPlanta, int idZona);
    }
}


