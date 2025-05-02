using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Viajes
{
    public class  ModelBuscaServicio
    {
        public int Idruta { get; set; }

        public string? Ruta { get; set; }

        public int Idparadainicial { get; set; }

        public string? Paradainicial { get; set; }

        public int Idparadafinal { get; set; }

        public string? Paradafinal { get; set; }

        public decimal? DistanciaInicial { get; set; }

        public decimal? DistanciaFinal { get; set; }
    }
}
