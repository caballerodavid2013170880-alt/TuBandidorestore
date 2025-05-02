using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Configuracion.Tarifas
{
    public class RutaTipoTarifaModel
    {
        public int RutaId { get; set; }

        public string? RutaNombre { get; set; }

        public int EmpresaId { get; set; }

        public string? EmpresaNombre { get; set; }

        public int? TipoTarifaId { get; set; }

        public string? TipoTarifaNombre { get; set; }
    }
    public class EmpresaModel
    {
        public int EmpresaId { get; set; }

        public string? Nombre { get; set; }



    }
    public class TipoTarifaModel
    {
        public int TipoTarifaId { get; set; }

        public string? Nombre { get; set; }


    }
    public class EmpresaTarifaModel
    {
        public int EmpresaId { get; set; }
        public int RutaId { get; set; }
        public int? TarifaId { get; set; }
        public string ParadaInicio { get; set; }
        public List<ParadaRutaModel> TarifaEscalonada { get; set; }
        public  TarifaGeneralModel TarifaGeneral { get; set; }
    }

    public class ParadaRutaModel
    {
        public int ParadaId { get; set; }
        public string NombreParada { get; set; }
        public int Orden { get; set; }
        public List<TarifaEscalonadaModel> Escalas { get; set; }
    }
    public class TarifaEscalonadaModel
    {
        public int ParadaInicio { get; set; }
        public int ParadaFin { get; set; }
        public decimal MontoPago { get; set;}
        public int Orden {  get; set; }

    }
    public class TarifaGeneralModel
    {
        public int? EmpresaId { get; set; }
        public int RutaId { get; set; }
        public decimal MontoTarifa { get; set; }

    }
}
