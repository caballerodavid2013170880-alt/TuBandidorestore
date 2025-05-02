using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Service.Contenidos
{
  public interface ITipoContenidoService
  {
    /// <summary>
    /// Obtiene todos los tipos de contenido desde la base de datos.
    /// </summary>
    /// <returns>Lista de todos los tipos de contenido.</returns>
    Task<List<Tipocontenido>> GetAll();
    /// <summary>
    /// Obtiene los tipos de contenido activos desde la base de datos.
    /// </summary>
    /// <returns>Lista de tipos de contenido activos.</returns>
    Task<List<Tipocontenido>> GetActive();
  }
}