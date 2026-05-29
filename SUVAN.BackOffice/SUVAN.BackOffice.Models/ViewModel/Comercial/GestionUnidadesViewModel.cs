using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Comercial
{
    public class GestionUnidadesViewModel
    {
        public GestionUnidadesViewModel GestionUnidades { get; set; }
        public GestionUnidadesViewModel()
        {
            this.GestionUnidades = new GestionUnidadesViewModel();
        }
    }

    public class ObtenerUnidadesViewModel
    {
        public int Idunidad { get; set; }
        public string? NombreUnidad { get; set; }
        public string? PlacasUnidad { get; set; }    
        public string? NumeroEcoUnidad { get; set; }
        public ulong? ActivoUnidad { get; set; }
    }

    public class ObtenerGeneralesUnidadesViewModel
    {
        public int Idunidad { get; set; }
        public string? TipoUnidad { get; set; }
        public string? PlacasUnidad { get; set; }
        public string? NumeroEcoUnidad { get; set; }
        public ulong? ActivoUnidad { get; set; }
        public string? Marca {  get; set; }
        public string? Modelo { get; set; }
        public string? NumeroMotor {  get; set; }
        public string? VIN {  get; set; }
    }

    public class AsignacionesUnidadViewModel
    {
        public int Idunidad { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public string? Conductor { get; set; }
        public string? Ruta { get; set; }
        public TimeOnly? Horario { get; set; }
    }

    public class SeguroUnidadViewModel
    {
        public int Idunidad { get; set; }
        public DateTime? FechaFinSeguro { get; set; }
        public string? NumeroPoliza { get; set; }
    }

}
