using System;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.ViewModel.Comercial;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;

namespace SUVAN.BackOffice.Service.Comercial
{
    public class OperadoresService : IOperadoresService
    {
        private readonly SuvanDbContext _context;
        public OperadoresService(SuvanDbContext context)
        {
            _context = context;
        }
        public async Task<List<ObtenerOperadoresViewModel>> ObtenerOperadores(int empresaId)
        {
            var result = await (from o in _context.Conductors
                                where o.EmpresaIdempresa == empresaId
                                orderby o.Idconductor descending
                                select new ObtenerOperadoresViewModel
                                {
                                    Idoperador = o.Idconductor,
                                    Nombre = o.Nombre,
                                    RFC = o.Rfc,
                                    Curp = o.Curp,
                                    INE = o.Ine,
                                    Licencia = o.Numerolicencia,
                                    TipoLicencia = o.Tipolicencia,
                                    Telefono = o.Telefono,
                                    Email = o.Email
                                }).ToListAsync();
            return result;
        }
        public async Task<ObtenerDetalleOperadorViewModel> ObtenerDetalleOperador(int operadorId)
        {
            var promedio = _context.CalificacionConductors
                .Where(c => c.ConductorIdconductor == operadorId) // Filtra por AlumnoId
                .Select(c => (long?)Convert.ToDecimal(c.Calificacion)) // Asegúrate de que la conversión a decimal? se hace aquí
                .DefaultIfEmpty() // Esto asegura que se devuelva un enumerable con un elemento 'null' si la secuencia está vacía
                .Average();
            var promedioFinal = promedio.GetValueOrDefault();
            promedio = promedioFinal;
            ObtenerDetalleOperadorViewModel result = new();
            result = await (from o in _context.Conductors
                            select new ObtenerDetalleOperadorViewModel()
                            {
                                Idusuario = o.Idconductor,
                                Nombre = o.Nombre,
                                Email = o.Email,
                                FotoUsuario = o.Imagen,
                                Calificacion = promedio.ToString()
                            }).FirstOrDefaultAsync(x => x.Idusuario == operadorId);
            return result;
        }

        public async Task<List<ObtenerViajesOperadorViewModel>> ObtenerViajesOperador(int operadorId)
        {

            DateTime fechaHoraActual = DateTime.Now;

            List<ObtenerViajesOperadorViewModel> model = new();
            model = await (from v in _context.Viajes
                .Include(v => v.ParadaInicioNavigation)
                .Include(v => v.ParadaFinNavigation)
                .Include(v => v.CorridaAsignacionIdcorridaAsignacionNavigation)
                .ThenInclude(c => c.CorridaIdcorridaNavigation)
                .ThenInclude(co => co.RutaIdrutaNavigation)
                .Include(v => v.CorridaAsignacionIdcorridaAsignacionNavigation)
                .ThenInclude(c => c.CorridaIdcorridaNavigation)
                .ThenInclude(co => co.CorridaParada)
                .Include(v => v.TransaccionIdtransaccionNavigation)
                           where v.CorridaAsignacionIdcorridaAsignacionNavigation.ConductorIdconductor == operadorId && v.Fechaviaje < fechaHoraActual
                           && !(new int[] { Convert.ToInt32(EnumEstatusViaje.CANCELADO) }).Contains(v.EstatusviajeIdestatusviaje)
                           && !(new int[] { Convert.ToInt32(EnumEstatusViaje.RESERVANDO) }).Contains(v.EstatusviajeIdestatusviaje)
                           orderby v.Idviaje descending
                           select new ObtenerViajesOperadorViewModel()
                           {
                               ViajeId = v.Idviaje,
                               Viaje = (v.ParadaInicioNavigation.Nombre ?? "") + " - " + (v.ParadaFinNavigation.Nombre ?? ""),
                               Ruta = v.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre,
                               Corrida = v.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre,
                               PuntoAbordaje = v.ParadaInicioNavigation.Nombre,
                               PuntoDescenso = v.ParadaFinNavigation.Nombre,
                               Fechaviaje = v.Fechaviaje,
                               Pasajeros = v.Numeropasajeros,
                               HoraAbordaje = v.ParadaInicioNavigation.CorridaParada.FirstOrDefault(cp => cp.ParadaIdparada == v.ParadaInicio && cp.CorridaIdcorrida == v.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.Idcorrida).Hora,
                               HoraDescenso = v.ParadaFinNavigation.CorridaParada.FirstOrDefault(cp => cp.ParadaIdparada == v.ParadaFin && cp.CorridaIdcorrida == v.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.Idcorrida).Hora,
                               Precio = "$" + v.TransaccionIdtransaccionNavigation.Cantidad.ToString()
                           }).ToListAsync();
            return model;
        }

        public async Task<List<ObtenerViajesOperadorViewModel>> ObtenerViajesProximosOperador(int operadorId)
        {

            DateTime fechaHoraActual = DateTime.Now;

            List<ObtenerViajesOperadorViewModel> model = new();
            model = await (from v in _context.Viajes
                .Include(v => v.ParadaInicioNavigation)
                .Include(v => v.ParadaFinNavigation)
                .Include(v => v.CorridaAsignacionIdcorridaAsignacionNavigation)
                .ThenInclude(c => c.CorridaIdcorridaNavigation)
                .ThenInclude(co => co.RutaIdrutaNavigation)
                .Include(v => v.CorridaAsignacionIdcorridaAsignacionNavigation)
                .ThenInclude(c => c.CorridaIdcorridaNavigation)
                .ThenInclude(co => co.CorridaParada)
                .Include(v => v.TransaccionIdtransaccionNavigation)
                           where v.CorridaAsignacionIdcorridaAsignacionNavigation.ConductorIdconductor == operadorId && v.Fechaviaje > fechaHoraActual
                           && (new int[] { Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) }).Contains(v.EstatusviajeIdestatusviaje)
                           orderby v.Idviaje descending
                           select new ObtenerViajesOperadorViewModel()
                           {
                               ViajeId = v.Idviaje,
                               Viaje = (v.ParadaInicioNavigation.Nombre ?? "") + " - " + (v.ParadaFinNavigation.Nombre ?? ""),
                               Ruta = v.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre,
                               Corrida = v.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre,
                               PuntoAbordaje = v.ParadaInicioNavigation.Nombre,
                               PuntoDescenso = v.ParadaFinNavigation.Nombre,
                               Fechaviaje = v.Fechaviaje,
                               Pasajeros = v.Numeropasajeros,
                               HoraAbordaje = v.ParadaInicioNavigation.CorridaParada.FirstOrDefault(cp => cp.ParadaIdparada == v.ParadaInicio && cp.CorridaIdcorrida == v.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.Idcorrida).Hora,
                               HoraDescenso = v.ParadaFinNavigation.CorridaParada.FirstOrDefault(cp => cp.ParadaIdparada == v.ParadaFin && cp.CorridaIdcorrida == v.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.Idcorrida).Hora,
                               Precio = "$" + v.TransaccionIdtransaccionNavigation.Cantidad.ToString()
                           }).ToListAsync();
            return model;
        }

        public async Task<List<CalificacionOperadorViewModel>> CalificacionOperador(int operadorId)
        {
            List<CalificacionOperadorViewModel> model = new();
            model = await (from c in _context.CalificacionConductors
                           where c.ConductorIdconductor == operadorId
                           orderby c.ViajeIdviaje descending
                           select new CalificacionOperadorViewModel()
                           {
                               ViajeId = c.ViajeIdviaje,
                               NombreUsuario = c.UsuarioIdusuarioNavigation.Nombre,
                               Calificacion = c.Calificacion,
                               Ruta = c.ViajeIdviajeNavigation.CorridaAsignacionIdcorridaAsignacionNavigation.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre,
                               PuntoAbordaje = c.ViajeIdviajeNavigation.ParadaInicioNavigation.Nombre,
                               PuntoDescenso = c.ViajeIdviajeNavigation.ParadaFinNavigation.Nombre,
                               HoraAbordaje = c.ViajeIdviajeNavigation.ParadaInicioNavigation.CorridaParada.FirstOrDefault(cp => cp.ParadaIdparada == c.ViajeIdviajeNavigation.ParadaInicio).Hora,
                               HoraDescenso = c.ViajeIdviajeNavigation.ParadaFinNavigation.CorridaParada.FirstOrDefault(cp => cp.ParadaIdparada == c.ViajeIdviajeNavigation.ParadaFin).Hora,
                               Fechaviaje = c.ViajeIdviajeNavigation.Fechaviaje
                           }).ToListAsync();

            return model;
        }

        public async Task<List<AsignacionesOperadorViewModel>> AsignacionesOperador(int operadorId)
        {
            List<AsignacionesOperadorViewModel> model = new();
            model = await (from c in _context.CorridaAsignacions
                           where c.ConductorIdconductor == operadorId
                           orderby c.IdcorridaAsignacion descending
                           select new AsignacionesOperadorViewModel()
                           {
                               Ruta = c.CorridaIdcorridaNavigation.RutaIdrutaNavigation.Nombre,
                               Horario = c.CorridaIdcorridaNavigation.HoraInicio,
                               Unidad = c.VehiculoIdvehiculoNavigation.Placas,
                               Fecha = c.Fecha
                           }).ToListAsync();
            return model;
        }

        public async Task<PagosOperadorViewModel> PagosOperador(int operadorId)
        {
            PagosOperadorViewModel vModel = new PagosOperadorViewModel();

            List<PagosOperadorVM> model = new();
            model = await (from lc in _context.LiquidacionCabeceras
                           where lc.Idconductor == operadorId
                           orderby lc.IdLiquidacion descending
                           select new PagosOperadorVM()
                           {
                               LiquidacionId = lc.IdLiquidacion,
                               operadorId = lc.Idconductor,
                               FechaEmision = lc.FechaEmision,
                               FechaInicio = lc.FechaInico,
                               FechaFin = lc.FechaFin,
                               MontoPagado = lc.MontoPagado ?? 0,
                               MontoComision = lc.MontoComision ?? 0
                           }).ToListAsync();
            vModel.lPagosOperador = model;

            return vModel;
        }
    }
}
