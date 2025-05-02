using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Contenidos
{
  public class TipoContenidoService : ITipoContenidoService
  {
    private readonly SuvanDbContext context;

    public TipoContenidoService(SuvanDbContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// Obtiene todos los tipos de contenido desde la base de datos.
    /// </summary>
    /// <returns>Lista de todos los tipos de contenido.</returns>
    public async Task<List<Tipocontenido>> GetAll()
    {
      return await context.Tipocontenidos.ToListAsync();
    }

    /// <summary>
    /// Obtiene los tipos de contenido activos desde la base de datos.
    /// </summary>
    /// <returns>Lista de tipos de contenido activos.</returns>
    public async Task<List<Tipocontenido>> GetActive()
    {

      return await context.Tipocontenidos
        .Where(x => x.Activo == 1)
        .ToListAsync();

    }
  }
}
