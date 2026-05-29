using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Utilities.Tools
{
  public static class Utilidades
  {
    /// <summary>
    /// Convierte la fecha del servidor a la fecha del cliente año mes dia
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ConvertirFechaServer(this string input)
    {
      string fechaServer = string.Empty;

      // invertir fecha de dd/mm/yyyy a yyyy/mm/dd
      string[] fecha = input.Trim().Split('/');
      fechaServer = $"{fecha[2]}/{fecha[1]}/{fecha[0]}";

      return fechaServer;

    }
  }
}
