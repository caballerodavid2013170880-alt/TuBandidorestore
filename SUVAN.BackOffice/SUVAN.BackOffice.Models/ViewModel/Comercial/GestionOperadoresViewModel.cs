using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Comercial
{
    public class GestionOperadoresViewModel
    {

        public GestionOperadoresViewModel GestionOperadores { get; set; }
        public GestionOperadoresViewModel()
        {
            this.GestionOperadores = new GestionOperadoresViewModel();
        }
    }

    public class ObtenerOperadoresViewModel
    {
        public int Idoperador { get; set; }

        public string? RFC { get; set; }

        public string? Nombre { get; set; }

        public string? Curp { get; set; }

        public string? INE { get; set; }

        public string? Licencia { get; set; }

        public string? TipoLicencia { get; set; }

        public string? Telefono { get; set; }

        public string? Email { get; set; }
    }

    public class ObtenerDetalleOperadorViewModel
    {
        public int Idusuario { get; set; }

        public string? Email { get; set; }

        public ulong? Activo { get; set; }

        public string? Nombre { get; set; }

        public string? FotoUsuario { get; set; }

        public string? Calificacion { get; set; }
    }
    public class CalificacionOperadorViewModel
    {
        public int ViajeId { get; set; }
        public DateTime? Fechaviaje { get; set; }
        public string? Ruta { get; set; }
        public string? PuntoAbordaje { get; set; }
        public TimeOnly? HoraAbordaje { get; set; }
        public string? PuntoDescenso { get; set; }
        public TimeOnly? HoraDescenso { get; set; }
        public string? NombreUsuario { get; set; }
        public int? Calificacion { get; set; }
    }

    public class ObtenerViajesOperadorViewModel
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

    public class PagosOperadorVM
    {
        public int LiquidacionId { get; set; }
        public int operadorId { get; set; }
        public DateTime FechaEmision {  get; set; }
        public DateTime FechaInicio {  get; set; }
        public DateTime FechaFin {  get; set; }
        public decimal MontoPagado {  get; set; }
        public decimal MontoComision {  get; set; }
    }

    public class PagosOperadorViewModel
    {
        public string? FechaInicio { get; set; } = string.Empty;
        public string? FechaFin { get; set; } = string.Empty;

        public List<PagosOperadorVM> lPagosOperador {  get; set; }
    }

    public class AsignacionesOperadorViewModel
    {
        public int IdAsignaciones { get; set; }
        public string? Ruta { get; set; }
        public TimeOnly? Horario { get; set; }
        public string? Unidad { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
