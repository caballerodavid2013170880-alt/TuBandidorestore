using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class PoliticasCompensacionService : IPoliticasCompensacionService
  {
    private readonly SuvanDbContext context;

    public PoliticasCompensacionService(SuvanDbContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// obtiene las politicas de compensacion de una empresa
    /// </summary>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<List<Politicascompensacion>> GetPoliticasCompensacion(int empresaId)
    {
      return await context.Politicascompensacions
        .Where(x => x.EmpresaIdempresa == empresaId && x.Tipocancelacion == (int)EnumTipoPoliticaCancelacion.Cliente)
        .ToListAsync();
    }


    /// <summary>
    /// obtiene una politica de compensacion por id y empresa
    /// </summary>
    /// <param name="id"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<AgregarPoliticaCompensacionViewModel> GetPoliticasCompensacionesViewModel(int id, int empresaId)
    {
      var politica = await context.Politicascompensacions
        .FirstOrDefaultAsync(x => x.EmpresaIdempresa == empresaId && x.Idpoliticacompensacion == id);

      if (politica == null)
        return new AgregarPoliticaCompensacionViewModel();

      var model = new AgregarPoliticaCompensacionViewModel()
      {
        PoliticaId = politica.Idpoliticacompensacion,
        EmpresaId = (int)politica.EmpresaIdempresa!,
        Descripcion = politica.Descripcion!,
        Activo = politica.Activa == 1,
        PorcentajeCompensacion = (decimal)politica.Porcentajecompensacion!,
        RangoTiempo = (decimal)politica.Rangotiempo!,
        TipoCancelacion = (int)EnumTipoPoliticaCancelacion.Cliente,
        TipoTiempo = (int)politica.Tipotiempo!,
        TipoPolitca = (int)politica.Tipopolitica!

      };

      return model;
    }


    /// <summary>
    /// agrega una politica a la BD
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> GuardarPoliticaCompensacion(AgregarPoliticaCompensacionViewModel model, int empresaId)
    {
      Politicascompensacion politica = new();
      if (model.PoliticaId > 0)
      {
        politica = await context.Politicascompensacions
          .FirstOrDefaultAsync(x => x.Idpoliticacompensacion == model.PoliticaId && x.EmpresaIdempresa == empresaId);

        if (politica == null)
        {
          throw new Exception("No existe información para actualizar");
        }
      }

      // validar que no exista una otra politica con el mismo tiempo por tipo de cancelacion
      var politicaExistente = await context.Politicascompensacions
        .FirstOrDefaultAsync(x => x.Rangotiempo == model.RangoTiempo
                               && x.Tipotiempo == model.TipoTiempo
                               && x.Tipocancelacion == (int)EnumTipoPoliticaCancelacion.Cliente
                               && x.Tipopolitica == model.TipoPolitca
                               && x.EmpresaIdempresa == empresaId
                               && x.Idpoliticacompensacion != model.PoliticaId);

      if (politicaExistente != null)
      {
        throw new Exception("Ya existe una política con el mismo rango de tiempo y tipo de política");
      }


      politica.Idpoliticacompensacion = model.PoliticaId;
      politica.EmpresaIdempresa = empresaId;
      politica.Descripcion = model.Descripcion;
      politica.Activa = (ulong?)(model.Activo ? 1 : 0);
      politica.Porcentajecompensacion = model.PorcentajeCompensacion;
      politica.Rangotiempo = model.RangoTiempo;
      politica.Tipocancelacion = (int)EnumTipoPoliticaCancelacion.Cliente;
      politica.Tipotiempo = model.TipoTiempo;
      politica.Fecharegistro = DateTime.Now;
      politica.Tipopolitica = model.TipoPolitca;


      if (politica.Idpoliticacompensacion == 0)
      {
        context.Politicascompensacions.Add(politica);
      }
      else
      {
        context.Politicascompensacions.Update(politica);
      }

      await context.SaveChangesAsync();

      return true;
    }


    /// <summary>
    /// obtiene las politicas de compensacion de una empresa
    /// </summary>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<List<Politicascompensacion>> GetPoliticasCompensacionEmpresa(int empresaId)
    {
      return await context.Politicascompensacions
        .Where(x => x.EmpresaIdempresa == empresaId && x.Tipocancelacion == (int)EnumTipoPoliticaCancelacion.Empresa)
        .ToListAsync();
    }


    /// <summary>
    /// obtiene una politica de compensacion por id y empresa
    /// </summary>
    /// <param name="id"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<AgregarPoliticaCompensacionViewModel> GetPoliticasCompensacionesEmpresaViewModel(int id, int empresaId)
    {
      var politica = await context.Politicascompensacions
        .FirstOrDefaultAsync(x => x.EmpresaIdempresa == empresaId && x.Idpoliticacompensacion == id);

      if (politica == null)
        return new AgregarPoliticaCompensacionViewModel();

      var model = new AgregarPoliticaCompensacionViewModel()
      {
        PoliticaId = politica.Idpoliticacompensacion,
        EmpresaId = (int)politica.EmpresaIdempresa!,
        Descripcion = politica.Descripcion!,
        Activo = politica.Activa == 1,
        PorcentajeCompensacion = (decimal)politica.Porcentajecompensacion!,
        RangoTiempo = (decimal)politica.Rangotiempo!,
        TipoCancelacion = (int)EnumTipoPoliticaCancelacion.Empresa,
        TipoTiempo = (int)politica.Tipotiempo!,
        TipoPolitca = (int)politica.Tipopolitica!

      };

      return model;
    }


    /// <summary>
    /// agrega una politica a la BD
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> GuardarPoliticaCompensacionEmpresa(AgregarPoliticaCompensacionViewModel model, int empresaId)
    {
      Politicascompensacion politica = new();
      if (model.PoliticaId > 0)
      {
        politica = await context.Politicascompensacions
          .FirstOrDefaultAsync(x => x.Idpoliticacompensacion == model.PoliticaId && x.EmpresaIdempresa == empresaId);

        if (politica == null)
        {
          throw new Exception("No existe información para actualizar");
        }
      }

      // validar que no exista una otra politica con el mismo tiempo por tipo de cancelacion
      var politicaExistente = await context.Politicascompensacions
        .FirstOrDefaultAsync(x => x.Rangotiempo == model.RangoTiempo
                               && x.Tipotiempo == model.TipoTiempo
                               && x.Tipocancelacion == (int)EnumTipoPoliticaCancelacion.Empresa
                               && x.Tipopolitica == model.TipoPolitca
                               && x.EmpresaIdempresa == empresaId
                               && x.Idpoliticacompensacion != model.PoliticaId);

      if (politicaExistente != null)
      {
        throw new Exception("Ya existe una política con el mismo rango de tiempo y tipo de política");
      }


      politica.Idpoliticacompensacion = model.PoliticaId;
      politica.EmpresaIdempresa = empresaId;
      politica.Descripcion = model.Descripcion;
      politica.Activa = (ulong?)(model.Activo ? 1 : 0);
      politica.Porcentajecompensacion = model.PorcentajeCompensacion;
      politica.Rangotiempo = model.RangoTiempo;
      politica.Tipocancelacion = (int)EnumTipoPoliticaCancelacion.Empresa;
      politica.Tipotiempo = model.TipoTiempo;
      politica.Fecharegistro = DateTime.Now;
      politica.Tipopolitica = model.TipoPolitca;


      if (politica.Idpoliticacompensacion == 0)
      {
        context.Politicascompensacions.Add(politica);
      }
      else
      {
        context.Politicascompensacions.Update(politica);
      }

      await context.SaveChangesAsync();

      return true;
    }
  }
}
