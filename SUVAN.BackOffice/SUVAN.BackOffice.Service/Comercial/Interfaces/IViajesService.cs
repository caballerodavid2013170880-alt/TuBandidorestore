using SUVAN.BackOffice.Models.ViewModel.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Comercial
{
  public interface IViajesService
  {
    public Task NotificarViajeCancelado(List<int> viajesId, int empresaId);
    public Task<ObtenerDetalleViajesViewModel> ObtenerDetalleViajes(int? corridaId);
    public Task<List<ObtenerViajesViewModel>> ObtenerViajes(int empresaId);
    public Task<List<ObtenerViajesViewModel>> ObtenerViajesProximos(int empresaId);
    public Task<List<ObtenerViajesViewModel>> ObtenerViajesCancelados(int empresaId);
    public Task<ObtenerDetalleViajesViewModel> ObtenerDetalleViajesUsuario(int? viajeId);
    public Task<List<ObtenerViajesUsuarioViewModel>> ObtenerViajesUsuario(int empresaId);
    public Task<List<ObtenerViajesUsuarioViewModel>> ObtenerViajesProximosUsuario(int empresaId);
    public Task<List<ObtenerViajesUsuarioViewModel>> ObtenerViajesCanceladosUsuario(int empresaId);
    public Task<string> CancelaViaje(int viajeId, int corridaAsignacionId, int empresaId);
  }
}

