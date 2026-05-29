using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class TarifaService : ITarifaService
  {
    private readonly SuvanDbContext _context;
    public TarifaService(SuvanDbContext context)
    {
      _context = context;
    }
    public List<EmpresaModel> ObtenerEmpresas(int rutaId)
    {
      var resul = (from o in _context.Empresas
                   join k in _context.Ruta on o.Idempresa equals k.EmpresaIdempresa
                   where k.Idruta == rutaId
                   select new EmpresaModel()
                   {
                     EmpresaId = o.Idempresa,
                     Nombre = o.Nombre
                   }).ToList();
      return resul;
    }

    public List<RutaTipoTarifaModel> ObtenerRutas(int empresaId)
    {
      var resul = (from a in _context.Ruta
                   where a.EmpresaIdempresa == empresaId
                   select new RutaTipoTarifaModel()
                   {
                     RutaId = a.Idruta,
                     RutaNombre = a.Nombre,
                     TipoTarifaId = a.TipotarifaIdtipotarifa
                   }).ToList();
      return resul;
    }

    public List<TipoTarifaModel> ObtenerTipoTarifa()
    {
      var resul = (from o in _context.Tipotarifas
                   select new TipoTarifaModel()
                   {
                     TipoTarifaId = o.Idtipotarifa,
                     Nombre = o.Nombre
                   }).ToList();
      return resul;
    }
    public EmpresaTarifaModel ObtenerPrecioTarifa(EmpresaTarifaModel model)
    {

      var resul = (from a in _context.Ruta
                   join b in _context.Empresas on a.EmpresaIdempresa equals b.Idempresa
                   join c in _context.Tipotarifas on a.TipotarifaIdtipotarifa equals c.Idtipotarifa
                   where a.Idruta == model.RutaId
                   select new RutaTipoTarifaModel()
                   {
                     RutaId = a.Idruta,
                     RutaNombre = a.Nombre,
                     EmpresaId = b.Idempresa,
                     EmpresaNombre = b.Nombre,
                     TipoTarifaId = c.Idtipotarifa,
                     TipoTarifaNombre = c.Nombre
                   }).FirstOrDefault();

      model.EmpresaId = resul.EmpresaId;

      if (model.TarifaId != null)
        model.TarifaId = model.TarifaId;
      else
        model.TarifaId = resul.TipoTarifaId;

      switch (model.TarifaId)
      {
        case 1:
          var tarifGeneral = _context.TarifaGenerals.Where(x => x.RutaIdruta == model.RutaId).FirstOrDefault();
          if (tarifGeneral == null)
          {
            TarifaGeneral tarifaGral = (from a in _context.Ruta
                                        where a.Idruta == model.RutaId
                                        select new TarifaGeneral()
                                        {
                                          RutaIdruta = a.Idruta,
                                          EmpresaIdempresa = (int)a.EmpresaIdempresa,
                                          Precio = 0
                                        }).FirstOrDefault();

            if (tarifaGral != null)
            {
              _context.TarifaGenerals.Add(tarifaGral);
              _context.SaveChanges();
            }
          }

          model.TarifaGeneral = _context.TarifaGenerals
                          .Where(x =>
                          x.EmpresaIdempresa == model.EmpresaId &&
                          x.RutaIdruta == model.RutaId)
                          .Select(o => new TarifaGeneralModel()
                          {
                            EmpresaId = o.EmpresaIdempresa,
                            RutaId = o.RutaIdruta,
                            MontoTarifa = o.Precio.Value
                          }).FirstOrDefault();

          break;

        case 2:
          var ruta = _context.RutaParada.Where(x => x.RutaIdruta == model.RutaId).OrderBy(x => x.Orden).First();
          var paradaInicio = _context.Parada.Where(x => x.Idparada == ruta.ParadaIdparada).First();
          if (paradaInicio != null)
            model.ParadaInicio = paradaInicio.Nombre;

          //Verifica si ya existen los registros de la tarifa escalonada
          var tarifEscalonada = _context.TarifaEscalonada.Where(x => x.RutaIdruta == model.RutaId).FirstOrDefault();
          if (tarifEscalonada == null)
          {
            List<TarifaEscalonadum> tarifasEscalonadas = (from a in _context.Ruta
                                                          join b in _context.RutaParada on a.Idruta equals b.RutaIdruta
                                                          join c in _context.RutaParada on a.Idruta equals c.RutaIdruta
                                                          where a.Idruta == model.RutaId && b.Orden < c.Orden
                                                          select new TarifaEscalonadum()
                                                          {
                                                            RutaIdruta = a.Idruta,
                                                            EmpresaIdempresa = a.EmpresaIdempresa ?? 0,
                                                            ParadaIdparada = b.ParadaIdparada,
                                                            ParadaIdparada1 = c.ParadaIdparada,
                                                            Precio = 0
                                                          }).ToList();

            foreach (var item in tarifasEscalonadas)
            {
              _context.TarifaEscalonada.Add(item);
              _context.SaveChanges();
            }
          }

          model.TarifaEscalonada = (from o in _context.Parada
                                    join k in _context.RutaParada on o.Idparada equals k.ParadaIdparada
                                    orderby k.Orden ascending
                                    where k.RutaIdruta == model.RutaId
                                    select new ParadaRutaModel()
                                    {
                                      ParadaId = o.Idparada,
                                      NombreParada = o.Nombre,
                                      Orden = k.Orden.Value,
                                      Escalas = (from m in _context.TarifaEscalonada
                                                 join j in _context.RutaParada on new { RutaID = m.RutaIdruta, ParadaID = m.ParadaIdparada } equals new { RutaID = j.RutaIdruta, ParadaID = j.ParadaIdparada }
                                                 where m.EmpresaIdempresa == model.EmpresaId &&
                                                 m.RutaIdruta == model.RutaId &&
                                                 m.ParadaIdparada1 == o.Idparada
                                                 select new TarifaEscalonadaModel()
                                                 {
                                                   ParadaInicio = m.ParadaIdparada,
                                                   ParadaFin = m.ParadaIdparada1,
                                                   MontoPago = m.Precio.HasValue ? m.Precio.Value : 0,
                                                   Orden = j.Orden ?? 0
                                                 }).ToList()
                                    }).ToList();
          break;

      }
      return model;
    }

    public async Task<EmpresaTarifaModel> ActualizaPrecioTarifa(EmpresaTarifaModel model)
    {
      var ruta = _context.Ruta.Where(x => x.Idruta == model.RutaId).FirstOrDefault();
      ruta.TipotarifaIdtipotarifa = model.TarifaId;
      _context.Update(ruta);
      _context.SaveChanges();

      switch (model.TarifaId)
      {
        case 1:
          _context.TarifaGenerals.Entry(new TarifaGeneral()
          {
            EmpresaIdempresa = (int)model.TarifaGeneral.EmpresaId,
            RutaIdruta = model.TarifaGeneral.RutaId,
            Precio = model.TarifaGeneral.MontoTarifa

          }).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          await _context.SaveChangesAsync();
          break;

        case 2:

          foreach (var tarifa in model.TarifaEscalonada)
          {

            if (tarifa.Escalas != null)
            {

              foreach (var escalas in tarifa.Escalas)
              {
                var tarifaEscala = await _context.TarifaEscalonada
                  .FirstOrDefaultAsync(x => x.RutaIdruta == model.RutaId
                  && x.EmpresaIdempresa == model.EmpresaId
                  && x.ParadaIdparada == escalas.ParadaInicio
                  && x.ParadaIdparada1 == escalas.ParadaFin);

                if (tarifaEscala != null)
                {

                  tarifaEscala.Precio = escalas.MontoPago;

                  _context.TarifaEscalonada.Update(tarifaEscala);
                }
              }

              await _context.SaveChangesAsync();
            }
          }


          //model.TarifaEscalonada.ForEach(x =>
          //{
          //  if (x.Escalas != null)
          //  {
          //    x.Escalas.ForEach(k =>
          //              {
          //                _context.TarifaEscalonada.Update(new TarifaEscalonadum()
          //                {
          //                  ParadaIdparada = k.ParadaInicio,
          //                  ParadaIdparada1 = k.ParadaFin,
          //                  EmpresaIdempresa = model.EmpresaId,
          //                  RutaIdruta = model.RutaId,
          //                  Precio = k.MontoPago

          //                });
          //                _context.SaveChanges();
          //                //_context.TarifaEscalonada.Entry(new TarifaEscalonadum()
          //                //{
          //                //  ParadaIdparada = k.ParadaInicio,
          //                //  ParadaIdparada1 = k.ParadaFin,
          //                //  EmpresaIdempresa = model.EmpresaId,
          //                //  RutaIdruta = model.RutaId,
          //                //  Precio = k.MontoPago

          //                //}).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          //              });

          //  }
          //});

          break;

      }
      return model;
    }
  }
}
