using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Viajes
{
    public class ReservacionViajeRequest
    {
        public int UserId { get; set; }
        public int CorridaId { get; set; }
        public int Pasajeros { get; set; }

        public int EstacionAbordajeId { get; set; }
        public int EstacionDescensoId { get; set; }

        public bool agregaviajeredondo { get; set; }
        public Viajeredondo viajeredondo { get; set; }
    }

    public class ReservacionViajeReponse
    {
        public int ReservacionId { get; set; }
    }

    public class Viajeredondo
    {
        public string origennombre { get; set; }
        //public int origenId { get; set; }
        public decimal? origenlatitud { get; set; }
        public decimal? origenlongitud { get; set; }
        public string origendireccion { get; set; }
        public string destinonombre { get; set; }
       //public int destinoId { get; set; }
        public decimal? destinolatitud { get; set; }
        public decimal? destinolongitud { get; set; }
        public string destinodireccion { get; set; }

    }

}
