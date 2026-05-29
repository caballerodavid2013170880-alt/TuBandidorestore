using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Models.ViewModel.Ingresos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Ingresos
{
  public class ReporteIngresosService : IReporteIngresosService
  {
    private readonly SuvanDbContext context;

    public ReporteIngresosService(SuvanDbContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// regresa los filtros para el reporte de ingresos
    /// </summary>
    /// <returns></returns>
    public async Task<ReporteIngresosViewModel> InitReporteIngresosGlobal()
    {
      var model = new ReporteIngresosViewModel();

      List<CatalogoIngresosViewModel> empresas = await GetEmpresas();
      List<CatalogoIngresosViewModel> metodosPago = await GetMetodoPago();

      model.Empresas = empresas;
      model.MetodoPago = metodosPago;

      return model;

    }


    /// <summary>
    /// Consulta el reporte de ingresos 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ReporteIngresosViewModel> ReporteIngresosSearch(ReporteIngresosViewModel model)
    {

      model.Empresas = await GetEmpresas();
      model.MetodoPago = await GetMetodoPago();
      DateTime desde = DateTime.Now;
      DateTime hasta = DateTime.Now;

      if (!string.IsNullOrEmpty(model.desdeview))
      {
        desde = (DateTime)ConvertDatePeriodo(model.desdeview, model.Periodo, true)!;
        hasta = (DateTime)ConvertDatePeriodo(model.hastaview!, model.Periodo, false)!;
      }


      var search = await (from v in context.Viajes
                          join t in context.Transaccions on v.TransaccionIdtransaccion equals t.Idtransaccion
                          join m in context.Metodopagos on t.MetodopagoIdmetodopago equals m.Idmetodopago
                          join p in context.Tipotransaccions on t.TipotransaccionIdtipotransaccion equals p.Idtipotransaccion
                          join e in context.Empresas on v.EmpresaIdempresa equals e.Idempresa
                          where (t.EstatustransaccionIdestatustransaccion == 2 || t.EstatustransaccionIdestatustransaccion == 5)
                          && (model.EmpresaId == null || v.EmpresaIdempresa == model.EmpresaId)
                          && (model.MetodoPagoId == null || t.MetodopagoIdmetodopago == model.MetodoPagoId)
                          && (string.IsNullOrEmpty(model.desdeview) || (t.Fecharegistro!.Value.Date >= ConvertDatePeriodo(model.desdeview, model.Periodo, true)
                          && t.Fecharegistro!.Value.Date <= ConvertDatePeriodo(model.hastaview!, model.Periodo, false)))
                          orderby t.Fecharegistro descending
                          select new ReporteIngresosDetalleViewModel
                          {
                            Fecha = t.Fecharegistro ?? Convert.ToDateTime(t.Fecharegistro),
                            Empresa = e.Nombre!,
                            MetodoPago = m.Nombre!,
                            TipoPago = p.Nombre!,
                            Cantidad = t.Cantidad ?? Convert.ToDecimal(t.Cantidad)
                          }
          ).ToListAsync();

      model.Detalle = search;

      if (!string.IsNullOrEmpty(model.desdeview))
      {
        model.Desde = desde.ToString("dd/MM/yyyy");
        model.Hasta = hasta.ToString("dd/MM/yyyy");
      }

      model.Total = search.Sum(x => x.Cantidad).ToString("C");

      return model;
    }



    /// <summary>
    /// Consulta el reporte de ingresos por emresa
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ReporteIngresosViewModel> ReporteIngresosEmpresaSearch(ReporteIngresosViewModel model)
    {

      model.Empresas = await GetEmpresas();
      model.MetodoPago = await GetMetodoPago();

      DateTime desde = DateTime.Now;
      DateTime hasta = DateTime.Now;

      if (!string.IsNullOrEmpty(model.desdeview))
      {
        desde = (DateTime)ConvertDatePeriodo(model.desdeview, model.Periodo, true)!;
        hasta = (DateTime)ConvertDatePeriodo(model.hastaview!, model.Periodo, false)!;
      }

      var search = await (from v in context.Viajes
                          join t in context.Transaccions on v.TransaccionIdtransaccion equals t.Idtransaccion
                          join m in context.Metodopagos on t.MetodopagoIdmetodopago equals m.Idmetodopago
                          join p in context.Tipotransaccions on t.TipotransaccionIdtipotransaccion equals p.Idtipotransaccion
                          join e in context.Empresas on v.EmpresaIdempresa equals e.Idempresa
                          where (t.EstatustransaccionIdestatustransaccion == 2 || t.EstatustransaccionIdestatustransaccion == 5)
                          && (t.TipotransaccionIdtipotransaccion == 1)
                          && (model.EmpresaId == null || v.EmpresaIdempresa == model.EmpresaId)
                          && (model.MetodoPagoId == null || t.MetodopagoIdmetodopago == model.MetodoPagoId)
                          && (string.IsNullOrEmpty(model.desdeview) || (t.Fecharegistro!.Value.Date >= ConvertDatePeriodo(model.desdeview, model.Periodo, true)
                          && t.Fecharegistro!.Value.Date <= ConvertDatePeriodo(model.hastaview!, model.Periodo, false)))
                          orderby t.Fecharegistro descending
                          select new ReporteIngresosDetalleViewModel
                          {
                            Fecha = t.Fecharegistro ?? Convert.ToDateTime(t.Fecharegistro),
                            Empresa = e.Nombre!,
                            MetodoPago = m.Nombre!,
                            TipoPago = p.Nombre!,
                            Cantidad = t.Cantidad ?? Convert.ToDecimal(t.Cantidad)
                          }
          ).ToListAsync();

      model.Detalle = search;


      if (!string.IsNullOrEmpty(model.desdeview))
      {
        model.Desde = desde.ToString("dd/MM/yyyy");
        model.Hasta = hasta.ToString("dd/MM/yyyy");
      }

      model.Total = search.Sum(x => x.Cantidad).ToString("C");

      return model;
    }

    /// <summary>
    /// calula la fecha de inicio y fin de acuerdo al periodo seleccionado
    /// </summary>
    /// <param name="date"></param>
    /// <param name="periodo"></param>
    /// <param name="isDesde"></param>
    /// <returns></returns>
    private DateTime? ConvertDatePeriodo(string date, EnumPeriodoReporte periodo, bool isDesde)
    {
      switch (periodo)
      {
        case EnumPeriodoReporte.Dia:
          var dateCast = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
          if (isDesde)
          {
            return dateCast.Date;
          }
          else
          {
            return dateCast.AddDays(1).AddSeconds(-1).Date;
          }
        case EnumPeriodoReporte.Mes:

          if (date.Length == 10)
          {
            date = date.Substring(3, 7);
          }

          DateTime fecha = DateTime.ParseExact($@"01/{date}", "dd/MM/yyyy", CultureInfo.InvariantCulture);
          if (isDesde)
          {
            return fecha.Date;
          }
          else
          {
            return fecha.AddMonths(1).AddDays(-1).Date;
          }
        case EnumPeriodoReporte.Anio:
          if (date.Length == 10)
          {
            date = date.Substring(6, 4);
          }

          DateTime fechaAnio = DateTime.ParseExact($@"01/01/{date}", "dd/MM/yyyy", CultureInfo.InvariantCulture);
          if (isDesde)
          {
            return fechaAnio.Date;
          }
          else
          {
            return fechaAnio.AddYears(1).AddDays(-1).Date;
          }

        default:
          return null;
      }
    }

    /// <summary>
    /// obtiene los metodos de pago registradas en la base de datos
    /// </summary>
    /// <returns></returns>
    private async Task<List<CatalogoIngresosViewModel>> GetMetodoPago()
    {
      return await context.Metodopagos.Select(x => new CatalogoIngresosViewModel
      {
        Id = x.Idmetodopago,
        Descripcion = x.Nombre!
      }).ToListAsync();
    }

    /// <summary>
    /// obtiene las empresa registradas en la base de datos
    /// </summary>
    /// <returns></returns>
    private async Task<List<CatalogoIngresosViewModel>> GetEmpresas()
    {
      return await context.Empresas
        .Select(x => new CatalogoIngresosViewModel
        {
          Id = x.Idempresa,
          Descripcion = x.Nombre!
        }).ToListAsync();
    }
  }
}
