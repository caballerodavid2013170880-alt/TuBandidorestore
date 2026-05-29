using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Viajes
{
    public class RecorridoViajeRequest
    {
        public int ReservacionId { get; set; }
    }

    public class EstacionModel
    {
        public string? Nombre { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public Int32 ViajeUsuario { get; set; }

    }

    public class RecorridoViajeResponse 
    {
        public string? GoogleMapsRuta {  get; set; }

        public List<EstacionModel>? Estaciones { get; set; }
    }

    public class RecorridoViajeOperadorRequest
    {
        public int CorridaAsignacionId { get; set; }
    }
}
