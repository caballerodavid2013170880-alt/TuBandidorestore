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
    public interface IPlantaService
    {
        /// <summary>
        /// Obtiene el listado de plantas de la empresa indicada,
        /// incluyendo la navegación a la entidad <see cref="Region"/>
        /// para poder mostrar el nombre de la región en la tabla de la vista.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// </param>
        /// <returns>
        /// Lista de entidades <see cref="Plantum"/> con la propiedad de navegación
        /// <c>Id</c> (Región) cargada.
        /// </returns>
        Task<List<Plantum>> GetPlantas(int idEmpresa);
        /// <summary>
        /// Construye y devuelve el <see cref="PlantaViewModel"/> necesario para
        /// renderizar el formulario de alta o edición de una planta.
        /// La lista de regiones incluida en el ViewModel se filtra por la empresa
        /// del usuario para respetar la jerarquía: empresa → región → planta.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// Determina qué regiones se muestran en el selector.
        /// </param>
        /// <param name="idPlanta">
        /// Identificador de la planta a editar.
        /// Pasar 0 para obtener un ViewModel vacío (modo alta).
        /// </param>
        /// <returns>
        /// ViewModel poblado con los datos de la planta (si existe) y
        /// la lista de regiones disponibles para el usuario.
        /// </returns>
        /// <exception cref="Exception">
        /// Si <paramref name="idPlanta"/> es mayor a 0 y la planta no existe
        /// o no pertenece a la empresa del usuario.
        /// </exception>
        Task<PlantaViewModel> GetPlantaViewModel(int idEmpresa, int idPlanta);
        /// <summary>
        /// Inserta o actualiza una planta en la base de datos.
        /// Aplica las siguientes validaciones de seguridad y negocio:
        /// <list type="bullet">
        ///   <item>La región seleccionada pertenece a la empresa del usuario.</item>
        ///   <item>En modo edición, la planta pertenece a la empresa del usuario.</item>
        ///   <item>No existe una planta con el mismo nombre en la misma región y empresa.</item>
        /// </list>
        /// </summary>
        /// <param name="model">ViewModel con los datos capturados en el formulario.</param>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// Se utiliza para validar la región y sobrescribir el campo empresa en la entidad.
        /// </param>
        /// <returns><c>true</c> si la operación fue exitosa.</returns>
        /// <exception cref="Exception">
        /// Si la región no pertenece a la empresa, la planta en edición no existe en la empresa,
        /// o existe un nombre duplicado en la misma región y empresa.
        /// </exception>
        Task<bool> AgregarPlanta(PlantaViewModel model, int idEmpresa);
    }

}
