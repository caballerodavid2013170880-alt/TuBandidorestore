using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.Contenido;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Models.Variables;

namespace SUVAN.BackOffice.Service.Contenidos
{
  public class ContenidoService : IContenidoService
  {
    private readonly SuvanDbContext context;

    public ContenidoService(SuvanDbContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// Obtiene todos los contenidos generales desde la base de datos.
    /// </summary>
    /// <returns>Lista de contenidos generales.</returns>
    public async Task<List<Contenido>> GetAllGeneral()
    {
      return await context.Contenidos
        .Where(x => x.TipocontenidoIdtipocontenido == (int)EnumTipoContenido.General)
        .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los contenidos de preguntas frecuentes desde la base de datos.
    /// </summary>
    /// <returns>Lista de contenidos de preguntas frecuentes ordenados por el campo Orden.</returns>
    public async Task<List<Contenido>> GetAllPreguntas()
    {
      return await context.Contenidos
        .Where(x => x.TipocontenidoIdtipocontenido == (int)EnumTipoContenido.PreguntasFrecuentes)
        .OrderBy(x => x.Orden)
        .ToListAsync();
    }

    /// <summary>
    /// Actualiza el orden de los contenidos.
    /// </summary>
    /// <param name="nuevoOrden">Lista que contiene el nuevo orden de los contenidos.</param>
    /// <returns>True si la operación fue exitosa.</returns>
    public async Task<bool> ActualizarOrden(List<int> nuevoOrden, EnumTipoContenido tipoContenido)
    {

      var contenidos = await context.Contenidos
        .Where(x => x.TipocontenidoIdtipocontenido == (int)tipoContenido)
        .ToListAsync();

      if (nuevoOrden != null)
      {
        for (int i = 0; i < nuevoOrden.Count; i++)
        {
          var contenido = contenidos.FirstOrDefault(x => x.Idcontenido == nuevoOrden[i]);
          if (contenido != null)
          {
            contenido.Orden = i + 1; // +1 porque los índices de DataTable comienzan desde 0
            contenido.Fecharegistro = DateTime.Now;
          }
        }

        await context.SaveChangesAsync();
      }

      return true;

    }

    /// <summary>
    /// Agrega o actualiza un contenido general o de preguntas frecuentes en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel con los datos del contenido.</param>
    /// <param name="tipoContenido">Tipo de contenido (general o preguntas frecuentes).</param>
    /// <returns>True si la operación fue exitosa.</returns>
    public async Task<bool> AgregarContenidoGeneral(AgregarContenidoViewModel model, EnumTipoContenido tipoContenido)
    {
      Contenido contenido = new();
      if (model.ContenidoId > 0)
      {
        contenido = await context.Contenidos
         .FirstOrDefaultAsync(x => x.Idcontenido == model.ContenidoId);

        if (contenido != null)
        {
          contenido.Titulo = model.Titulo;
          contenido.Html = model.Contenido;
          if (model.Imagen != null)
          {
            contenido.Imagen = model.Imagen.GetBase64();

          }
          contenido.Activo = (ulong?)(model.Activo ? 1 : 0);
          contenido.Orden = tipoContenido == EnumTipoContenido.General ? 1 : model.Orden;
          contenido.TipocontenidoIdtipocontenido = (int)tipoContenido;
          contenido.Fecharegistro = DateTime.Now;

          context.Contenidos.Entry(contenido);
        }
      }
      else
      {
        // agrega el orden a una nueva pregunta frecuente
        if (EnumTipoContenido.PreguntasFrecuentes == tipoContenido)
        {
          var orden = await context.Contenidos
            .Where(x => x.TipocontenidoIdtipocontenido == (int)tipoContenido)
            .OrderByDescending(x => x.Orden).ToListAsync();

          model.Orden = orden != null ? orden.Count + 1 : 1;
        }

        contenido = new Contenido
        {
          Titulo = model.Titulo,
          Html = model.Contenido,
          Activo = (ulong?)(model.Activo ? 1 : 0),
          Orden = tipoContenido == EnumTipoContenido.General ? 1 : model.Orden,
          TipocontenidoIdtipocontenido = (int)tipoContenido,
          Fecharegistro = DateTime.Now
        };

        if (model.Imagen != null)
        {
          contenido.Imagen = model.Imagen.GetBase64();
        }

        context.Contenidos.Add(contenido);

      }

      await context.SaveChangesAsync();

      return true;
    }
    /// <summary>
    /// Obtiene el ViewModel para el contenido específico.
    /// </summary>
    /// <param name="id">Identificador del contenido.</param>
    /// <returns>ViewModel para el contenido específico.</returns>
    public async Task<AgregarContenidoViewModel> GetContenidoViewModel(int id)
    {
      var contenido = await context.Contenidos
        .FirstOrDefaultAsync(x => x.Idcontenido == id);

      if (contenido == null)
        return new AgregarContenidoViewModel();

      return new AgregarContenidoViewModel
      {
        ContenidoId = contenido.Idcontenido,
        Titulo = contenido.Titulo!,
        Contenido = contenido.Html!,
        Imagen64 = contenido.Imagen!,
        Activo = contenido.Activo == 1,
        Orden = contenido.Orden!,
        TipoContenidoId = contenido.TipocontenidoIdtipocontenido
      };

    }

    /// <summary>
    /// Elimina un contenido por id.
    /// </summary>
    /// <param name="contenidoId">Identificador del contenido a eliminar.</param>
    /// <returns>True si la operación fue exitosa.</returns>
    public async Task<bool> EliminarContenido(int contenidoId)
    {
      var contenido = await context.Contenidos
        .FirstOrDefaultAsync(x => x.Idcontenido == contenidoId);

      if (contenido == null)
        return false;

      //contenido.Activo = 0;
      //contenido.Fecharegistro = DateTime.Now;

      //context.Contenidos.Entry(contenido);

      context.Contenidos.Remove(contenido);

      await context.SaveChangesAsync();

      return true;
    }

    /// <summary>
    /// Obtiene un contenido por id.
    /// </summary>
    /// <param name="contenidoId">Identificador del contenido.</param>
    /// <returns>Respuesta con el contenido especificado.</returns>
    public async Task<SuVanResponse<ContenidoResponse>> ObtenContenidoPorId(int contenidoId)
    {
      SuVanResponse<ContenidoResponse> response = new();

      var result = await (from o in context.Contenidos
                          where o.Idcontenido == contenidoId
                          select new ContenidoResponse()
                          {
                            Id = o.Idcontenido,
                            TipoContenido = o.TipocontenidoIdtipocontenido,
                            Titulo = o.Titulo,
                            Html = o.Html,
                            Imagen = o.Imagen,
                            Orden = o.Orden
                          }).FirstOrDefaultAsync();

      response.Data = result!;
      response.CodigoMensaje = "200";
      response.Mensaje = "Contenido exitoso";

            if (result == null)
      {
        response.Mensaje = "No se encontró Contenido";
      }


      return response;
    }

    /// <summary>
    /// Obtiene una lista de contenidos por tipo de contenido.
    /// </summary>
    /// <param name="tipoContenido">Tipo de contenido.</param>
    /// <returns>Respuesta con la lista de contenidos especificados.</returns>
    public async Task<SuVanResponse<List<ContenidoResponse>>> ObtenContenidoPorTipo(int tipoContenido)
    {
      SuVanResponse<List<ContenidoResponse>> response = new();

      var result = await (from o in context.Contenidos
                          where o.TipocontenidoIdtipocontenido == tipoContenido
                          select new ContenidoResponse()
                          {
                            Id = o.Idcontenido,
                            TipoContenido = o.TipocontenidoIdtipocontenido,
                            Titulo = o.Titulo,
                            Html = o.Html,
                            Imagen = o.Imagen,
                            Orden = o.Orden
                          }).ToListAsync();

      response.Data = result;
      response.CodigoMensaje = "200";

      if (result.Count() == 0)
      {
        response.Mensaje = "No se encontraron Contenidos";
      }

      return response;
    }

        /// <summary>
        /// ObtenContenidoGeneral
        /// </summary>
        /// <returns>Respuesta con la lista de contenidos generales</returns>
        public async Task<SuVanResponse<List<ContenidoGeneral>>> ObtenContenidosGenerales()
        {
            SuVanResponse<List<ContenidoGeneral>> response = new();

            //Se agregan 3 contenidos generales principales
            List <ContenidoGeneral> contenidos = new List<ContenidoGeneral>();
            for (int i = 1; i <= 3; i++)
            {
                ContenidoGeneral contenido = new ContenidoGeneral();
                contenido.Id = i;

                switch (i) 
                {
                    case 1:
                        contenido.Titulo = "Aviso de privacidad";
                        contenido.Coleccion = false;
                        contenido.Tipo = 1;
                        contenido.Imagen = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAb1JREFUSEu1VUtOwzAQHQ+04haEQxS6DCdpewuIkJpKKOIWTU/SsitwCMItUKE2HsuOJo4deRGysmL7vfF78xHwz59IxZ/MiqUQsKbzSsHm56OqU+4OElzNHzIpcQEASwDIPMBG/6tP79VmiChIwIBLdtkAKgVf9iWO0PxHlLvv4wutO1+HwAArXIMyEbuvCUlCkgGohRAitwcbEHCQAne/x+eDu9whmN4WigGXsah4iKGgtGwtbpCAH0gxks5YWT9pPSoBATvtnQKjEVzOn3KUcq+jLimbRiVg4FQXK6qL0QhC4KR9CgGZlOnsuSFdKRX9io2BJxFMZo97ymuJeI9KLkAB5XhbrUPg7Z6A+vRWrWJ1QL2mJE2FgGta6zZhKlWTvlpDW815CttetYUUAndIa2oILYlpDc5Qvz6md8WWOoC/328VEsmHRiKuqOQZSRScF5nzLyiRMcpGwp9KJNTkYi06dCdKwKJJasfc+FCLCbbr1jAmVagncXBXzYPtmm96BvcGC/fGzxyOMzjROiAAZi6oC2xQyq2bcLGsinrgP5HN4t7ITJnNSUOfjD+fMbcTLEsBTn5B6sCJnfsDKCBVKGwTHQUAAAAASUVORK5CYII=";
                        break;
                    case 2:
                        contenido.Titulo = "Terminos y condiciones";
                        contenido.Coleccion = false;
                        contenido.Tipo = 1;
                        contenido.Imagen = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAOdJREFUSEtjZKAxYKSx+Qz0tYDFosaB6d+/+QwMDApE+OwBUN2CX6faGvGpRfEBq2nlfkZGRgciDEdW0oDPEhQL2Myq/oN0AjUQDDpW06oERkYGkG9BAKclZFsAMpUYS/BaAPMRtiCD+ZKQJRRbgO6T//8ZEn+fblsAcxRFQYTsMySfPPh1qk2R6haADMSWSMgOImTXw+Jj8FlAYoYjPYhobgG+fDAaBwQLO2zxQzCZkllco9j1////A79PtztizcmgCofx7996MuoEmHkP/jExJf450XIAqwWkJkti1JMV1sQYTDcfAAAoSK0ZneHfxAAAAABJRU5ErkJggg==";
                        break;
                    case 3:
                        contenido.Titulo = "Preguntas frecuentes";
                        contenido.Coleccion = true;
                        contenido.Tipo = 2;
                        contenido.Imagen = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAaJJREFUSEu1lVFuwjAMhu0Iqh2jHALGG+Ukg1ts1aSBNHW7BdyE8sbgEMsxJqDJ4q7p0rRpGyHyBK3j77d/N0G488I754dOwGD6GjEhZlLKCBEjAOCAkIIELhjbXw/vaZtIJ+Bh+hxmGW6KpG05OGNi/nP45E1BjYBgEr+p4JVH+yj59nxM1vaeGsCRnEsJ68sp2f5VxiJEIBGhlXBlQyoA2iwE+7Y28fMxGdnKitidBSFflqYvFcBw/LKzey4lLEl5YfaGQDpJ8Ywg5qoIKgHDcbxAhDyBuRgTIzLQgudJHBWTgLmuogT4GKtGNr2cPuYkRO2TDcNQevEPeIw3arYXfSZHK3RVbQowKyBz7amo8XRyR/91fOmDHwBhe/5Klq7em2rUuOa5DZPrE+QyPOhqZyGkAuhjslYVTOKudtZN7uhpH+/LGP3tVCrwONxaYeYEVQD0p8s8o0VNs5+DzY+sBqAHrtmmdz0A7YedUcWT+u1zXNPWWvLGCnSDPTzxv3BMyBUGIWYiRAYzkEBXJkgpuTp105uuTK+5bAnuvPRvBf0CXnT2GfjvG64AAAAASUVORK5CYII=";
                        break;
                }
                contenidos.Add(contenido);
            }

            response.Data = contenidos;
            response.CodigoMensaje = "200";

            return response;
        }

        public async Task<SuVanResponse<ContenidoMembresiaResponse>> ObtenContenidoMembresia()
        {
            SuVanResponse<ContenidoMembresiaResponse> response = new();

            ContenidoMembresiaResponse? result = new ContenidoMembresiaResponse();
            result.contenidoresponse = new List<ContenidoResponse>();
            result.InfoVariables = new List<infoVariables>();

            var resultPortada = await (from o in context.Contenidos
                                where o.Idcontenido == 3//Portada
                                select new ContenidoResponse()
                                {
                                    Id = o.Idcontenido,
                                    TipoContenido = o.TipocontenidoIdtipocontenido,
                                    Titulo = o.Titulo,
                                    Html = o.Html,
                                    Imagen = o.Imagen,
                                    Orden = o.Orden
                                }).FirstOrDefaultAsync();
            result.contenidoresponse.Add(resultPortada);

            var resultInterior = await (from o in context.Contenidos
                                where o.Idcontenido == 4//Interior
                                select new ContenidoResponse()
                                {
                                    Id = o.Idcontenido,
                                    TipoContenido = o.TipocontenidoIdtipocontenido,
                                    Titulo = o.Titulo,
                                    Html = o.Html,
                                    Imagen = o.Imagen,
                                    Orden = o.Orden
                                }).FirstOrDefaultAsync();

            result.contenidoresponse.Add(resultInterior);


            var resultVariables = await (from v in context.Variables
                                join tv in context.Tipovariables on v.TipovariableIdtipovariable equals tv.Idtipovariable
                                join vg in context.Variableglobals on v.Idvariable equals vg.VariableIdvariable
                                select new infoVariables()
                                {
                                    codigo = v.Codigo,
                                    valor = vg.Valor
                                }).ToListAsync();
            result.InfoVariables = resultVariables;


            response.Data = result;
            response.CodigoMensaje = "200";


            return response;
        }
    }
}
