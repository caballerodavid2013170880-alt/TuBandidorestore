using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.StoredsProcedures
{
  public class ModelsStoredsProcedures
  {
    public class ModelstpBuscaServicio
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

    public class ModelstpRevisaEstacionalaRedonda
    {
      public int? idparada { get; set; }

      public string? nombre { get; set; }

      public decimal? latitud { get; set; }

      public decimal? longitud { get; set; }
    }

    public class ModelRutaConfiguracion
    {
      public int idruta { get; set; }
      public string nombre { get; set; } = null!;
      public bool corrida { get; set; } = false;
      public bool estacion { get; set; } = false;
      public bool asignacion { get; set; } = false;
      public bool tarifaEsc { get; set; } = false;
      public bool tarifaGen { get; set; } = false;

    }

  }
}
