using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class ParadasService : IParadasService
  {
    private readonly SuvanDbContext context;

    public ParadasService(SuvanDbContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// Obtiene el listado de estaciones desde la base de datos.
    /// </summary>
    /// <returns>Listado de estaciones.</returns>
    public async Task<List<Paradum>> GetParadas()
    {
      var paradas = await context.Parada.ToListAsync();
      return paradas!;
    }

    /// <summary>
    /// obtine el listado de estaciones de la base de datos por nombre y que esten activas
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<List<Paradum>> GetParadasByName(string name)
    {
      var paradas = await context.Parada
        .Where(x => x.Nombre!.Contains(name) && x.Activo == 1)
        .ToListAsync();
      return paradas!;
    }

    /// <summary>
    /// Obtiene el ViewModel para la estación específica.
    /// </summary>
    /// <param name="id">Identificador de la estación.</param>
    /// <returns>ViewModel de la estación.</returns>
    public async Task<AgregarEstacionViewModel> GetEstacionViewModel(int id)
    {
      var estacion = await context.Parada
        .FirstOrDefaultAsync(x => x.Idparada == id);

      if (estacion == null)
        return new AgregarEstacionViewModel();

      return new AgregarEstacionViewModel
      {
        EstacionId = estacion.Idparada,
        NombreEstacion = estacion.Nombre!,
        Latitud = (decimal)estacion.Latitud!,
        Longitud = (decimal)estacion.Longitud!,
        Calle = estacion.Calle!,
        Numero = estacion.Numero!,
        Municipio = estacion.Municipio!,
        Ciudad = estacion.Ciudad!,
        CodigoPostal = estacion.Codigopostal!,
        Colonia = estacion.Colonia!,
        Activo = estacion.Activo == 1
      };

    }


    /// <summary>
    /// Agrega o actualiza una estación en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel de la estación a agregar o actualizar.</param>
    /// <returns>Indica si la operación fue exitosa.</returns>
    /// <exception cref="Exception">Excepción lanzada en caso de errores.</exception>
    public async Task<bool> AgregarEstacion(AgregarEstacionViewModel model)
    {
      Paradum parada = new();

      if (model.EstacionId > 0)
      {
        parada = await context.Parada.FirstOrDefaultAsync(x => x.Idparada == model.EstacionId);

        if (parada == null)
          throw new Exception("No se encontro la estación");
      }

      // validate if existe one empresa with the same name and rfc and is not the same empresa
      var paradaExistente = await context.Parada
        .FirstOrDefaultAsync(x => (x.Nombre == model.NombreEstacion || (x.Longitud == model.Longitud && x.Latitud == model.Latitud)) && x.Idparada != model.EstacionId);

      if (paradaExistente != null)
      {
        throw new Exception("Ya existe una Estación con el mismo nombre ó la misma Latitud y Longitud");
      }

      if (model.Longitud == 0 || model.Latitud == 0)
      {
        throw new Exception("La latitud y longitud no pueden ser 0");
      }

      parada.Nombre = model.NombreEstacion.Trim();
      parada.Latitud = model.Latitud!;
      parada.Longitud = model.Longitud!;
      parada.Calle = model.Calle;
      parada.Numero = model.Numero;
      parada.Municipio = model.Municipio;
      parada.Ciudad = model.Ciudad;
      parada.Codigopostal = model.CodigoPostal;
      parada.Colonia = model.Colonia;
      parada.Activo = (ulong)(model.Activo ? 1 : 0);
      parada.Fecharegistro = DateTime.Now;


      if (model.EstacionId > 0)
      {
        context.Parada.Entry(parada);
      }
      else
      {
        context.Parada.Add(parada);
      }

      await context.SaveChangesAsync();

      return true;
    }

  }
}
