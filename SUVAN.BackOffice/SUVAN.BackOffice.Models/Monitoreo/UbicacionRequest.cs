using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Ubicacion
{
    public class RegistraUbicacionRequest
    {
        public int IdcorridaAsignacion { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class UbicacionResponse
    {
        public int IdcorridaAsignacion { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
