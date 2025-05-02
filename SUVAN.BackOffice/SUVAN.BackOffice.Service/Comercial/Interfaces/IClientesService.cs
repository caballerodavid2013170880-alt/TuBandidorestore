using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Comercial
{
    public interface IClientesService
    {
        public Task<List<ObtenerClientesViewModel>> ObtenerClientes();
        public Task<ObtenerDetalleClienteViewModel> ObtenerDetalleCliente(int UsuarioId);
        public Task<List<ObtenerViajesClienteViewModel>> ObtenerViajesCliente(int clienteId);
        public Task<List<ObtenerViajesClienteViewModel>> ObtenerViajesProximosCliente(int clienteId);
        public Task<List<MonederoClienteViewModel>> MonederoCliente(int clienteId);
        public Task<List<CalificacionClienteViewModel>> CalificacionCliente(int clienteId);
        public Task<List<MembresiaClienteViewModel>> MembresiaCliente(int clienteId);
        public Task<byte[]> EdoCuentaMonederoCliente(int clinteId);
    }
}
