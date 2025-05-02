using System;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.ViewModel.Comercial;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;


namespace SUVAN.BackOffice.Service.Comercial
{
    public class UnidadesService : IUnidadesService
    {
        private readonly SuvanDbContext _context;
        public UnidadesService(SuvanDbContext context)
        {
            _context = context;
        }

        public async Task<List<ObtenerUnidadesViewModel>> ObtenerUnidades(int empresaId)
        {
            var result = await(from u in _context.Vehiculos
                               where u.EmpresaIdempresa == empresaId
                               orderby u.Idvehiculo descending
                               select new ObtenerUnidadesViewModel
                               {
                                   Idunidad = u.Idvehiculo,
                                   NombreUnidad = u.TipovehiculoIdtipovehiculoNavigation.Nombre,
                                   PlacasUnidad = u.Placas,
                                   NumeroEcoUnidad = u.Numeroeconomico,
                                   ActivoUnidad = u.Activo
                               }).ToListAsync();
            return result;
        }

        public async Task<ObtenerUnidadesViewModel> ObtenerDetalleUnidad(int unidadId)
        {
            var result = await (from u in _context.Vehiculos
                                select new ObtenerUnidadesViewModel
                                {
                                    Idunidad = u.Idvehiculo,
                                    NombreUnidad = u.TipovehiculoIdtipovehiculoNavigation.Nombre,
                                    PlacasUnidad = u.Placas,
                                    NumeroEcoUnidad = u.Numeroeconomico,
                                    ActivoUnidad = u.Activo
                                }).FirstOrDefaultAsync(x => x.Idunidad == unidadId);
            return result;

        }
        public async Task<List<ObtenerGeneralesUnidadesViewModel>> GeneralesUnidad(int unidadId)
        {
            List<ObtenerGeneralesUnidadesViewModel> model = new();
            model = await(from v in  _context.Vehiculos
                          where v.Idvehiculo == unidadId
                          orderby v.Idvehiculo descending
                          select new ObtenerGeneralesUnidadesViewModel
                          {
                              TipoUnidad = v.TipovehiculoIdtipovehiculoNavigation.Nombre,
                              PlacasUnidad = v.Placas,
                              Marca = v.Marca,
                              Modelo = v.Modelo,
                              NumeroEcoUnidad = v.Numeroeconomico,
                              NumeroMotor = v.Numeromotor,
                              VIN = v.Vin
                          }).ToListAsync();
                return model;
        }

        public async Task<List<AsignacionesUnidadViewModel>> AsignacionesUnidad(int unidadId)
        {
            List<AsignacionesUnidadViewModel> model = new();
            model = await(from  u in _context.CorridaAsignacions
                          where u.VehiculoIdvehiculo == unidadId
                          orderby u.IdcorridaAsignacion descending
                          select new  AsignacionesUnidadViewModel
                          {
                              Idunidad = u.VehiculoIdvehiculo,
                              Conductor = u.ConductorIdconductorNavigation.Nombre,
                              FechaAsignacion = u.Fecha,
                              Horario = u.CorridaIdcorridaNavigation.HoraInicio,
                              Ruta = u.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre
                          }).ToListAsync();
            return model;

        }

        public async Task<List<SeguroUnidadViewModel>> SeguroUnidad(int unidadId)
        {
            List<SeguroUnidadViewModel> model = new();
            model = await (from u in _context.Vehiculos
                           where u.Idvehiculo == unidadId
                           orderby u.Idvehiculo descending
                           select new SeguroUnidadViewModel
                           {
                               Idunidad = u.Idvehiculo,
                               FechaFinSeguro = u.Fechafinseguro,
                               NumeroPoliza = u.Numeropoliza
                           }).ToListAsync();
            return model;

        }
    }
}
