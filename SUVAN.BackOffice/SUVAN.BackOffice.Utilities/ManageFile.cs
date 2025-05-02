using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Utilities
{
  public static class ManageFile
  {
    /// <summary>
    /// convierte una imagen de tipo IFromFile en base64
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static string GetBase64(this IFormFile file)
    {
      if (file == null)
      {
        return string.Empty;
      }

      using (var ms = new MemoryStream())
      {
        file.CopyTo(ms);
        var fileBytes = ms.ToArray();
        return Convert.ToBase64String(fileBytes);
      }
    }
  }
}
