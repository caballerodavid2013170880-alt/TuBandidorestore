using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Mensajeria
{
  public class MensajeConversacion
  {
    public int? ConversacionId { get; set; }
    public int TipoConversacion { get; set; }
    public string NombreConversacion { get; set; }
    public int EmpresaId { get; set; }
    public int? RutaId { get; set; }
    public int? OperadorId { get; set; }
    public int? UsuarioIdCreacion { get; set; }
    public string ConexionId { get; set; }
    public string Mensaje { get; set; }
  }

  public class CrearConversacion
  {
    //public string? Mensaje { get; set; }

    public int UsuarioAdminId { get; set; }

    //public string? ConexionId { get; set; }

  }

  public class CerrarConversacionModel
  {
    public int ConversacionId { get; set; }

  }

  public class AdministradorConversacionModel
  {
    public int AdministradorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
  }
}
