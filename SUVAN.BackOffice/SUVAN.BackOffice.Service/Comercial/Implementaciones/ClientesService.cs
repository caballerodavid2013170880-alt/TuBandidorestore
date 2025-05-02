using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.ViewModel.Comercial;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;

namespace SUVAN.BackOffice.Service.Comercial
{
    public class ClientesService : IClientesService
    {
        private readonly SuvanDbContext _context;
        public ClientesService(SuvanDbContext context)
        {
            _context = context;
        }
        public async Task<List<ObtenerClientesViewModel>> ObtenerClientes()
        {
            var result = await (from o in _context.Usuarios
                                orderby o.Idusuario descending
                                select new ObtenerClientesViewModel
                                {
                                    Idusuario = o.Idusuario,
                                    Nombre = o.Nombre,
                                    Email = o.Email,
                                }).ToListAsync();
            return result;
        }
        public async Task<ObtenerDetalleClienteViewModel> ObtenerDetalleCliente(int usuarioId)
        {
            var promedio = _context.CalificacionUsuarios
                .Where(c => c.UsuarioIdusuario == usuarioId)
                .Select(c => (long?)Convert.ToDecimal(c.Calificacion))
                .DefaultIfEmpty() 
                .Average();
            var promedioFinal = promedio.GetValueOrDefault();
            promedio = promedioFinal;
            ObtenerDetalleClienteViewModel result = new();
            result = await (from o in _context.Usuarios
                    .Include(x => x.Fotografium)
                    .Include(x => x.Membresia)
                    .Include(x => x.Viajes)
                            select new ObtenerDetalleClienteViewModel()
                            {
                                Idusuario = o.Idusuario,
                                Nombre = o.Nombre,
                                Email = o.Email,
                                FotoUsuario = o.Fotografium.Imagen,
                                Calificacion = promedio.ToString()
                            }).FirstOrDefaultAsync(x => x.Idusuario == usuarioId);
            return result;
        }

        public async Task<List<ObtenerViajesClienteViewModel>> ObtenerViajesCliente(int clienteId)
        {

            DateTime fechaHoraActual = DateTime.Now;

            List<ObtenerViajesClienteViewModel> model = new();
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
                           where v.UsuarioIdusuario == clienteId && v.Fechaviaje < fechaHoraActual
                           && !(new int[] { Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) }).Contains(v.EstatusviajeIdestatusviaje)
                           && !(new int[] { Convert.ToInt32(EnumEstatusViaje.RESERVANDO) }).Contains(v.EstatusviajeIdestatusviaje)
                           orderby v.Idviaje descending
                           select new ObtenerViajesClienteViewModel()
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

        public async Task<List<ObtenerViajesClienteViewModel>> ObtenerViajesProximosCliente(int clienteId)
        {

            DateTime fechaHoraActual = DateTime.Now;

            List<ObtenerViajesClienteViewModel> model = new();
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
                           where v.UsuarioIdusuario == clienteId && v.Fechaviaje > fechaHoraActual
                           && (new int[] { Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) }).Contains(v.EstatusviajeIdestatusviaje)
                           orderby v.Idviaje descending
                           select new ObtenerViajesClienteViewModel()
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

        public async Task<List<MonederoClienteViewModel>> MonederoCliente(int clienteId)
        {
            var saldoFinal = _context.Monederos
                .Where(c => c.UsuarioIdusuario == clienteId)
                .Select(c => c.Saldo).FirstOrDefault();
            List<MonederoClienteViewModel> model = new();
            model = await (from t in _context.Transaccions
                           where t.UsuarioIdusuario == clienteId
                           orderby t.Idtransaccion descending
                           select new MonederoClienteViewModel()
                           {
                               Idusuario = clienteId,
                               TransaccionId = t.Idtransaccion,
                               Fechatransaccion = t.Fecharegistro,
                               _Transaccion = t.EstatustransaccionIdestatustransaccionNavigation.Nombre,
                               MetodoPago = t.MetodopagoIdmetodopagoNavigation.Nombre,
                               Cantidad = "$" + t.Cantidad.ToString(),
                               TipoTransaccion = t.TipotransaccionIdtipotransaccionNavigation.Nombre,
                               SaldoFinal = "$" + saldoFinal.ToString(),
                           }).ToListAsync();

            return model;
        }

        public async Task<byte[]> EdoCuentaMonederoCliente(int clienteId)
        {
            var saldoFinal = _context.Monederos
                .Where(c => c.UsuarioIdusuario == clienteId)
                .Select(c => c.Saldo).FirstOrDefault();

            var NombreUsuario = _context.Usuarios
                .Where(u => u.Idusuario == clienteId)
                .Select(u => u.Nombre).FirstOrDefault();

            List<EdoCuentaMonederoClienteViewModel> Transaction = new();
            Transaction = await (from t in _context.Transaccions
                           where t.UsuarioIdusuario == clienteId && t.EstatustransaccionIdestatustransaccion == 2
                                  && !(t.TipotransaccionIdtipotransaccion == 1 && t.Numeroordenpay != null && t.Numeropeticionpay != null)
                                 orderby t.Fecharegistro ascending
                                 select new EdoCuentaMonederoClienteViewModel()
                           {
                               Idusuario = clienteId,
                               TransaccionId = t.Idtransaccion,
                               Fechatransaccion = t.Fecharegistro.ToString(),
                               _Transaccion = t.EstatustransaccionIdestatustransaccionNavigation.Nombre,
                               MetodoPago = t.MetodopagoIdmetodopagoNavigation.Nombre,
                               Cantidad = "$" + t.Cantidad.ToString(),
                               TipoTransaccion = t.TipotransaccionIdtipotransaccionNavigation.Nombre,
                           }).ToListAsync();


            string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
            string HtmlContent = File.ReadAllText($@"{directorioBase}Plantilla\EdoCuentaMonederoPlantilla.html");

            // Construir las filas de la tabla
            StringBuilder tableRows = new StringBuilder();
            foreach (EdoCuentaMonederoClienteViewModel transaction in Transaction)
            {
                DateTime fecha = DateTime.Now;
                if (DateTime.TryParse(transaction.Fechatransaccion, out fecha))
                {
                    transaction.Fechatransaccion = fecha.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    tableRows.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", transaction.Fechatransaccion, transaction.TipoTransaccion, transaction.MetodoPago, transaction.Cantidad);
                }
            }

            // Reemplazo de los marcadores de posición
            string htmlContent = HtmlContent
                .Replace("{{Nombre del Usuario}}", NombreUsuario)
                .Replace("{{filasTabla}}", tableRows.ToString())
                .Replace("{{saldoFinal}}", "$" + saldoFinal.ToString());

            byte[]? bytesPDF = Utilities.PDF.getBytesPDF(htmlContent);
            return bytesPDF;
        }

        public async Task<List<CalificacionClienteViewModel>> CalificacionCliente(int clienteId)
        {
            List<CalificacionClienteViewModel> model = new();
            model = await (from c in _context.CalificacionUsuarios
                           where c.UsuarioIdusuario == clienteId
                           orderby c.ViajeIdviaje descending
                           select new CalificacionClienteViewModel()
                           {
                               ViajeId = c.ViajeIdviaje,
                               NombreOperador = c.ConductorIdconductorNavigation.Nombre,
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

        public async Task<List<MembresiaClienteViewModel>> MembresiaCliente(int clienteId)
        {

            List<MembresiaClienteViewModel> model = new();
            model = await (from c in _context.Membresia
                           where c.UsuarioIdusuario == clienteId
                           orderby c.Idmembreria descending
                           select new MembresiaClienteViewModel()
                           {
                               MembresiaId = c.Idmembreria,
                               Membresia = c.TransaccionIdtransaccionNavigation.TipotransaccionIdtipotransaccionNavigation.Nombre,
                               Costo = "$" + c.TransaccionIdtransaccionNavigation.Cantidad.ToString(),
                               Desde = c.Desde,
                               Hasta = c.Hasta
                           }).ToListAsync();
            return model;
        }
    }
}
