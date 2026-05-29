using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Comercial;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Comercial;
using SUVAN.BackOffice.Service.Conductor;
using static SUVAN.BackOffice.Models.UnlimintPay.Pago.RespuestaValidacionPagoWS;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class ComercialController : Controller
  {
    private readonly ILogger<ComercialController> _logger;
    private readonly IPromocionesService _promocionesService;
    private readonly IViajesService _viajesService;
    private readonly IClientesService _clientesService;
    private readonly IOperadoresService _operadoresService;
    private readonly IUnidadesService _unidadesService;
    private readonly IConductorService _conductorService;
    private const string _claimUserId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    public ComercialController(ILogger<ComercialController> logger, IPromocionesService promocionesService, IViajesService viajesService, IClientesService clientesService,
        IOperadoresService operadoresService, IUnidadesService unidadesService, IConductorService conductorService)
    {
      _logger = logger;
      _promocionesService = promocionesService;
      _viajesService = viajesService;
      _clientesService = clientesService;
      _operadoresService = operadoresService;
      _unidadesService = unidadesService;
      _conductorService = conductorService;
    }
    public async Task<IActionResult> CodigosPromocion()
    {
      var result = await _promocionesService.ConsultaPromocionesAsync(User.GetEmpresaId());
      return View(result);
    }
    [HttpGet]
    public async Task<IActionResult> GeneraPromocion([FromQuery] int? numeroPromocion)
    {
      var userPerfil = User.Claims.FirstOrDefault(i => i.Type == _claimUserId)!.Value;
      var resulEmpresaUsuario = await _promocionesService.ConsultaEmpresaUsuarioAsync(int.Parse(userPerfil));
      GeneraPromocionViewModel model = new GeneraPromocionViewModel()
      {
        Promocion = await _promocionesService.ConsultaPromocionAsync(numeroPromocion),
        TipoDescuento = await _promocionesService.TipoDescuentoAsync(),
        TipoPromocion = await _promocionesService.TipoPromocionAsync(),
        RutaCorridas = await _promocionesService.ConsultaRutaCorridaAsync(resulEmpresaUsuario.EmpresaId)

      };

      model.Promocion.EmpresaId = resulEmpresaUsuario.EmpresaId;
      model.Promocion.NombreEmpresa = resulEmpresaUsuario.NombreEmpresa;
      return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> GeneraPromocion(GeneraPromocionViewModel model)
    {
      var dataResponse = await _promocionesService.GeneraPromocionAsync(model.Promocion);
      return Json(dataResponse);

    }

    [HttpDelete]
    public async Task<IActionResult> RemuevePromocion([FromBody] PromocionesViewModel param)
    {
      var result = await _promocionesService.EliminaPromocionAsync(param);
      return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> Viajes()
    {
      //var result = await _viajesService.ObtenerViajes(User.GetEmpresaId());
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> Viajess()
    {
      var result = await _viajesService.ObtenerViajes(User.GetEmpresaId());
      return PartialView("_viajes", result);
    }

    [HttpGet]
    public async Task<IActionResult> ViajesUsuario()
    {
      var result = await _viajesService.ObtenerViajesUsuario(User.GetEmpresaId());
      return PartialView("_viajesUsuario", result);
    }

    [HttpGet]
    public async Task<IActionResult> ViajesProximos()
    {
      var result = await _viajesService.ObtenerViajesProximos(User.GetEmpresaId());
      return PartialView("_viajesProximos", result);
    }

    [HttpGet]
    public async Task<IActionResult> ViajesProximosUsuario()
    {
      var result = await _viajesService.ObtenerViajesProximosUsuario(User.GetEmpresaId());
      return PartialView("_viajesProximosUsuario", result);
    }

    [HttpGet]
    public async Task<IActionResult> ViajesCancelados()
    {
      var result = await _viajesService.ObtenerViajesCancelados(User.GetEmpresaId());
      return PartialView("_viajesCancelados", result);
    }

    [HttpGet]
    public async Task<IActionResult> ViajesCanceladosUsuario()
    {
      var result = await _viajesService.ObtenerViajesCanceladosUsuario(User.GetEmpresaId());
      return PartialView("_viajesCanceladosUsuario", result);
    }

    [HttpPost]
    public async Task<IActionResult> CancelarViaje(int idRuta, int corridaAsignacionId)
    {
      try
      {
        var result = await _viajesService.CancelaViaje(idRuta, corridaAsignacionId, User.GetEmpresaId());
        return Ok(new { success = true, message = result });
      }
      catch (Exception ex)
      {

        return Ok(new { success = false, message = ex.Message });
      }

    }

    [HttpGet]
    public async Task<IActionResult> DetalleViaje(int CorridaId)
    {
      var result = await _viajesService.ObtenerDetalleViajes(CorridaId);
      return PartialView("_DetalleViaje", result);
    }

    [HttpGet]
    public async Task<IActionResult> DetalleViajeUsuario(int CorridaId)
    {
      var result = await _viajesService.ObtenerDetalleViajesUsuario(CorridaId);
      return PartialView("_DetalleViajeUsuario", result);
    }

    [HttpGet]
    public async Task<IActionResult> Clientes()
    {
      var result = await _clientesService.ObtenerClientes();
      return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> DetalleCliente(int clienteId)
    {
      var result = await _clientesService.ObtenerDetalleCliente(clienteId);
      return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> ViajesCliente(int clienteId)
    {
      var result = await _clientesService.ObtenerViajesCliente(clienteId);
      return PartialView("_viajesClientes", result);
    }

    [HttpGet]
    public async Task<IActionResult> ViajesProximosCliente(int clienteId)
    {
      var result = await _clientesService.ObtenerViajesProximosCliente(clienteId);
      return PartialView("_viajesProximosClientes", result);
    }

    [HttpGet]
    public async Task<IActionResult> MonederoCliente(int clienteId)
    {
      var result = await _clientesService.MonederoCliente(clienteId);
      return PartialView("_monederoCliente", result);
    }

    [HttpGet]
    public async Task<IActionResult> CalificacionCliente(int clienteId)
    {
      var result = await _clientesService.CalificacionCliente(clienteId);
      return PartialView("_calificacionCliente", result);
    }

    [HttpGet]
    public async Task<IActionResult> MembresiaCliente(int clienteId)
    {
      var result = await _clientesService.MembresiaCliente(clienteId);
      return PartialView("_membresiaCliente", result);
    }

    [HttpGet]
    public async Task<IActionResult> Operadores()
    {
      var result = await _operadoresService.ObtenerOperadores(User.GetEmpresaId());
      return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> DetalleOperador(int operadorId)
    {
      var result = await _operadoresService.ObtenerDetalleOperador(operadorId);
      return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> ViajesOperador(int operadorId)
    {
      var result = await _operadoresService.ObtenerViajesOperador(operadorId);
      return PartialView("_viajesOperadores", result);
    }

    [HttpGet]
    public async Task<IActionResult> ViajesProximosOperador(int operadorId)
    {
      var result = await _operadoresService.ObtenerViajesProximosOperador(operadorId);
      return PartialView("_viajesProximosOperadores", result);
    }

    [HttpGet]
    public async Task<IActionResult> CalificacionOperador(int operadorId)
    {
      var result = await _operadoresService.CalificacionOperador(operadorId);
      return PartialView("_calificacionOperador", result);
    }

    [HttpGet]
    public async Task<IActionResult> AsignacionesOperador(int operadorId)
    {
      var result = await _operadoresService.AsignacionesOperador(operadorId);
      return PartialView("_asignacionesOperador", result);
    }

    [HttpGet]
    public async Task<IActionResult> PagosOperador(int operadorId)
    {
      var result = await _operadoresService.PagosOperador(operadorId);
      ViewBag.operadorId = operadorId;
      return PartialView("_pagosOperador", result);
    }

    public async Task<IActionResult> EmiteReciboPDF(int liquidacionID)
    {
      var documento = await _conductorService.EmiteRecibo(liquidacionID);
      return File(documento, "application/pdf", "ReciboLiquidacion_" + liquidacionID + ".pdf");
    }

    [HttpPost]
    public async Task<IActionResult> GeneraRecibo(GeneraReciboViewModel model)
    {
      try
      {
        int vLiquidacionId = await _conductorService.GeneraRecibo(model.operadorId, model.fechaInicio, model.fechaFin);
        return Json(new { success = true, LiquidacionID = vLiquidacionId });
      }
      catch (Exception e)
      {
        return Json(new { success = false, messge = e.Message });
      }
    }

    [HttpGet]
    public async Task<IActionResult> Unidades()
    {
      var result = await _unidadesService.ObtenerUnidades(User.GetEmpresaId());
      return View(result);

    }

    [HttpGet]
    public async Task<IActionResult> DetalleUnidad(int unidadId)
    {
      var result = await _unidadesService.ObtenerDetalleUnidad(unidadId);
      return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> GeneralesUnidad(int unidadId)
    {
      var result = await _unidadesService.GeneralesUnidad(unidadId);
      return PartialView("_generalesUnidad", result);
    }

    [HttpGet]
    public async Task<IActionResult> AsignacionesUnidad(int unidadId)
    {
      var result = await _unidadesService.AsignacionesUnidad(unidadId);
      return PartialView("_asignacionesUnidad", result);
    }

    [HttpGet]
    public async Task<IActionResult> SeguroUnidad(int unidadId)
    {
      var result = await _unidadesService.SeguroUnidad(unidadId);
      return PartialView("_seguroUnidad", result);
    }
    public async Task<IActionResult> ExportarPDF(int clienteId)
    {
      var transacciones = await _clientesService.EdoCuentaMonederoCliente(clienteId);
      return File(transacciones, "application/pdf", "EstadoDeCuenta.pdf");
    }
  }
}
