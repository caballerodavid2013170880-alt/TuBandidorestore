using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Utilities
{
  public class GeneraCodigos
  {

    public static string GeneraCodigo()
    {
      Random random = new();
      return random.Next(1000, 9999).ToString();
    }

    public static async Task<string> GetGeneraCodigo(int MaximoRango)
    {
      Random random = new Random();
      return random.Next(1000, MaximoRango).ToString();
    }
    
    
    public static async Task<string> GetGeneraCodigoAlfa()
    {
        string abc = "ABCDEFGHIJKLMONOPQRSTUVWXYZ";
        int longitud = 4; 
        char[] random = new char[longitud];

        for (int i = 0; i < longitud; i++)
        {
                random[i] = abc[RandomNumberGenerator.GetInt32(0, abc.Length)];
        }

        string code = new string(random) + await GetGeneraCodigo(9999);

        return code;
    }


    public static DateTime ExpiracionCodigoActivacion()
    {
      return DateTime.Now.AddMinutes(10);
    }


    public static string GeneraCodigoDescuento()
    {
      Random random = new();
      const string caracteresPermitidos = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      const int longitudCodigo = 6;

      StringBuilder codigoGenerado = new StringBuilder();

      for (int i = 0; i < longitudCodigo; i++)
      {
        char caracter = caracteresPermitidos[random.Next(caracteresPermitidos.Length)];
        codigoGenerado.Append(caracter);
      }

      return codigoGenerado.ToString();
    }
  }
}
