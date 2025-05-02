using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Parada
{
    public class ParadaCorrida
    {
        public int idRuta { get; set; }
        
        public int idParada {  get; set; }

        public string nombre { get; set; }

        public string parada {  get; set; }

        public int? orden {  get; set; }

        public decimal? longitud { get; set; }

        public decimal? latitud { get; set; }

    }
}
