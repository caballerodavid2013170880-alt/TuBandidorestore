using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Comercial
{
    public class GestionClientesViewModel
    {
        public GestionClientesViewModel GestionClientes { get; set; }
        public GestionClientesViewModel()
        {
            this.GestionClientes = new GestionClientesViewModel();
        }
    }

    public class ObtenerClientesViewModel
    {
        public int Idusuario { get; set; }

        public string? Email { get; set; }

        public string? Nombre { get; set; }
    }

    public class ObtenerDetalleClienteViewModel
    {
        public int Idusuario { get; set; }

        public string? Email { get; set; }

        public ulong? Activo { get; set; }

        public string? Nombre { get; set; }

        public string? FotoUsuario { get; set; }

        public string? Calificacion { get; set; }
    }

    public class ObtenerViajesClienteViewModel
    {
        public int ViajeId { get; set; }
        public string? Viaje { get; set; }
        public string? Ruta { get; set; }
        public DateTime? Fechaviaje { get; set; }
        public string? PuntoAbordaje { get; set; }
        public TimeOnly? HoraAbordaje { get; set; }
        public string? PuntoDescenso { get; set; }  
        public TimeOnly? HoraDescenso { get; set; }
        public string? Corrida { get; set; }
        public string? Precio { get; set; }
        public int? Pasajeros { get; set; }
        public int IdRuta { get; set; }
    }

    public class MonederoClienteViewModel
    {
        public int Idusuario { get; set; }
        public int TransaccionId { get; set; }
        public DateTime? Fechatransaccion { get; set; }
        public string? _Transaccion { get; set; }
        public string? MetodoPago { get; set; }
        public string? Cantidad { get; set; }
        public string? TipoTransaccion { get; set; }
        public string? SaldoFinal { get; set; }
    }
    public class CalificacionClienteViewModel
    {
        public int ViajeId { get; set; }
        public DateTime? Fechaviaje { get; set; }
        public string? Ruta { get; set; }
        public string? PuntoAbordaje { get; set; }
        public TimeOnly? HoraAbordaje { get; set; }
        public string? PuntoDescenso { get; set; }
        public TimeOnly? HoraDescenso { get; set; }
        public string? NombreOperador { get; set; }
        public int? Calificacion {  get; set; }
    }

    public class MembresiaClienteViewModel
    {
        public int MembresiaId { get; set; }
        public string? Membresia { get; set; }
        public string? Costo { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }

    public class EdoCuentaMonederoClienteViewModel
    {
        public int Idusuario { get; set; }
        public string? NombreUsuario { get; set; }
        public int TransaccionId { get; set; }
        public string? Fechatransaccion { get; set; }
        public string? _Transaccion { get; set; }
        public string? MetodoPago { get; set; }
        public string? Cantidad { get; set; }
        public string? TipoTransaccion { get; set; }
        public string? SaldoFinal {  get; set; }
    }
}
