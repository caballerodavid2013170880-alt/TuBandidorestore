using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Utilities;
using SUVAN.BackOffice.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class CodigoDescuentoService : ICodigoDescuentoService
  {
    private readonly SuvanDbContext context;
    private readonly INotificacionCorreoService notificacionCorreoService;

    public CodigoDescuentoService(SuvanDbContext context, INotificacionCorreoService notificacionCorreoService)
    {
      this.context = context;
      this.notificacionCorreoService = notificacionCorreoService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<List<Codigodescuento>> GetCodigoDescuentos()
    {
      var coigoDescuentos = await context.Codigodescuentos
        .OrderByDescending(x => x.Idcodigodescuento)
        .ToListAsync();

      return coigoDescuentos!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<AgregarDescuentoViewModel> GetCodigoDescuentoViewModel(int id)
    {
      var codigoDescuento = await context.Codigodescuentos
        .FirstOrDefaultAsync(x => x.Idcodigodescuento == id);

      if (codigoDescuento == null)
      {
        AgregarDescuentoViewModel newModel = new();
        newModel.Codigo = await ObtenerCodigo();
        return newModel;
      }

      var model = new AgregarDescuentoViewModel
      {
        DescuentoId = codigoDescuento.Idcodigodescuento,
        Codigo = codigoDescuento.Codigo!,
        Activo = codigoDescuento.Activo == 1,
        Cantidad = (decimal)codigoDescuento.Cantidad!,
        Desde = (DateTime)codigoDescuento.Vigenciadesde!,
        Hasta = (DateTime)codigoDescuento.Vigenciahasta!,
        Vigencia = $"{codigoDescuento.Vigenciadesde:dd/MM/yyyy} - {codigoDescuento.Vigenciahasta:dd/MM/yyyy}",
        TipoDescuento = codigoDescuento.TipodescuentoIdtipodescuento1,
        TipoCodigo = codigoDescuento.TipocodigodescuentoIdtipocodigodescuento
      };

      if (codigoDescuento.TipocodigodescuentoIdtipocodigodescuento == (int)EnumTipoCodigo.Exclisivo)
      {
        var correos = await context.Codigocorreos
          .Where(x => x.CodigodescuentoIdcodigodescuento == codigoDescuento.Idcodigodescuento)
          .Select(x => x.Email)
          .ToListAsync();

        model.Correos = string.Join("\r\n", correos);
      }

      return model;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> GuardarCodigoDescuento(AgregarDescuentoViewModel model)
    {
      Codigodescuento codigodescuento = new();
      if (model.DescuentoId > 0)
      {
        codigodescuento = await context.Codigodescuentos
          .FirstOrDefaultAsync(x => x.Idcodigodescuento == model.DescuentoId);
        if (codigodescuento is null)
        {
          throw new Exception("No existe información para actualizar");
        }
      }

      codigodescuento.Activo = (ulong?)(model.Activo ? 1 : 0);
      codigodescuento.Cantidad = model.Cantidad;
      codigodescuento.Vigenciadesde = Convert.ToDateTime(model.Vigencia.Split("-")[0].ConvertirFechaServer());
      codigodescuento.Vigenciahasta = Convert.ToDateTime(model.Vigencia.Split("-")[1].ConvertirFechaServer());
      codigodescuento.TipodescuentoIdtipodescuento1 = (int)model.TipoDescuento;
      codigodescuento.TipocodigodescuentoIdtipocodigodescuento = (int)model.TipoCodigo;
      codigodescuento.Fecharegistro = DateTime.UtcNow;

      if (model.DescuentoId > 0)
      {
        context.Codigodescuentos.Update(codigodescuento);
      }
      else
      {
        codigodescuento.Codigo = model.Codigo;
        context.Codigodescuentos.Add(codigodescuento);
      }

      await context.SaveChangesAsync();

      if (model.TipoCodigo == (int)EnumTipoCodigo.Exclisivo && !string.IsNullOrEmpty(model.Correos))
      {
        model.CorreosExclusivos = model.Correos.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        await GuardarCorreos(model.CorreosExclusivos, codigodescuento.Idcodigodescuento);
      }
      else
      {
        await EliminaCorreosExistentes(codigodescuento.Idcodigodescuento);
      }

      // envia correo si esta activo, es vigente y es de tipo exclusivo
      if (model.Activo
        && codigodescuento.TipocodigodescuentoIdtipocodigodescuento == (int)EnumTipoCodigo.Exclisivo
        && codigodescuento.Vigenciahasta >= DateTime.Today
        && !string.IsNullOrEmpty(model.Correos))
      {

        await EnvioCorreoDescuento(codigodescuento, model.CorreosExclusivos);
      }

      return true;
    }


    /// <summary>
    /// envia todos los correos de los usuarios que tienen el codigo de descuento
    /// </summary>
    /// <param name="codigodescuento"></param>
    /// <param name="correosExclusivos"></param>
    /// <returns></returns>
    private async Task EnvioCorreoDescuento(Codigodescuento codigodescuento, List<string> correosExclusivos)
    {

      foreach (var correo in correosExclusivos)
      {
        string descuento = codigodescuento.TipodescuentoIdtipodescuento1 == (int)EnumTipoDescuento.Porcentaje ? $"{codigodescuento.Cantidad}% de descuento" : $"{codigodescuento.Cantidad:C} de descuento";

        await notificacionCorreoService.EnviarCodigoDescuentoPortal(correo.Trim(), codigodescuento.Codigo!, descuento, (DateTime)codigodescuento.Vigenciahasta!);
      }

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="correos"></param>
    /// <param name="descuentoId"></param>
    /// <returns></returns>
    private async Task GuardarCorreos(List<string> correos, int descuentoId)
    {
      await EliminaCorreosExistentes(descuentoId);

      foreach (var correo in correos)
      {
        Codigocorreo codigoCorreo = new();
        codigoCorreo.CodigodescuentoIdcodigodescuento = descuentoId;
        codigoCorreo.Email = correo;
        context.Codigocorreos.Add(codigoCorreo);
      }

      await context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="descuentoId"></param>
    /// <returns></returns>
    private async Task EliminaCorreosExistentes(int descuentoId)
    {
      var correosExistentes = await context.Codigocorreos
        .Where(x => x.CodigodescuentoIdcodigodescuento == descuentoId)
        .ExecuteDeleteAsync();
    }


    /// <summary>
    /// Valida si el codigo de descuento existe en la base de datos.
    /// </summary>
    /// <returns></returns>
    private async Task<string> ObtenerCodigo()
    {
      while (true)
      {
        string codigo = GeneraCodigos.GeneraCodigoDescuento();

        var codigoDescuento = await context.Codigodescuentos
          .FirstOrDefaultAsync(x => x.Codigo == codigo);

        if (codigoDescuento == null)
        {
          return codigo;
        }
      }
    }
  }
}
