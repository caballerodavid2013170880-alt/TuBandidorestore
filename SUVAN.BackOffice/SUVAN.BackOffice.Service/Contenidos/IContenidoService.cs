using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Models.Contenido;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Contenidos
{
    public interface IContenidoService
    {

        /// <summary>
        /// Actualiza el orden de los contenidos.
        /// </summary>
        /// <param name="nuevoOrden">Lista que contiene el nuevo orden de los contenidos.</param>
        /// <returns>True si la operación fue exitosa.</returns>
        Task<bool> ActualizarOrden(List<int> nuevoOrden, EnumTipoContenido tipoContenido);
        /// <summary>
        /// Agrega o actualiza un contenido general o de preguntas frecuentes en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del contenido.</param>
        /// <param name="tipoContenido">Tipo de contenido (general o preguntas frecuentes).</param>
        /// <returns>True si la operación fue exitosa.</returns>
        Task<bool> AgregarContenidoGeneral(AgregarContenidoViewModel model, EnumTipoContenido tipoContenido);
        /// <summary>
        /// Elimina un contenido por id.
        /// </summary>
        /// <param name="contenidoId">Identificador del contenido a eliminar.</param>
        /// <returns>True si la operación fue exitosa.</returns>
        Task<bool> EliminarContenido(int contenidoId);
        /// <summary>
        /// Obtiene todos los contenidos generales desde la base de datos.
        /// </summary>
        /// <returns>Lista de contenidos generales.</returns>
        Task<List<Contenido>> GetAllGeneral();
        /// <summary>
        /// Obtiene todos los contenidos de preguntas frecuentes desde la base de datos.
        /// </summary>
        /// <returns>Lista de contenidos de preguntas frecuentes ordenados por el campo Orden.</returns>
        Task<List<Contenido>> GetAllPreguntas();
        /// <summary>
        /// Obtiene el ViewModel para el contenido específico.
        /// </summary>
        /// <param name="id">Identificador del contenido.</param>
        /// <returns>ViewModel para el contenido específico.</returns>
        Task<AgregarContenidoViewModel> GetContenidoViewModel(int id);
        /// <summary>
        /// Obtiene un contenido por id.
        /// </summary>
        /// <param name="contenidoId">Identificador del contenido.</param>
        /// <returns>Respuesta con el contenido especificado.</returns>
        Task<SuVanResponse<ContenidoResponse>> ObtenContenidoPorId(int contenidoId);

        /// <summary>
        /// Obtiene una lista de contenidos por tipo de contenido.
        /// </summary>
        /// <param name="tipoContenido">Tipo de contenido.</param>
        /// <returns>Respuesta con la lista de contenidos especificados.</returns>
        Task<SuVanResponse<List<ContenidoResponse>>> ObtenContenidoPorTipo(int tipoContenido);

        Task<SuVanResponse<List<ContenidoGeneral>>> ObtenContenidosGenerales();

        Task<SuVanResponse<ContenidoMembresiaResponse>> ObtenContenidoMembresia();

    }
}
