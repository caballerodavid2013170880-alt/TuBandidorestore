using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Mensajeria
{
  public class ConversacionModel
  {
    public int? ConversacionId { get; set; }
    public int TipoConversacion { get; set; }
    public int? EmpresaId { get; set; }
    public int? RutaId { get; set; }
    public int? OperadorId { get; set; }
    public string NombreConversacion { get; set; }
    public bool ConversacionCerrada { get; set; }
    public List<UsuariosConversacion> Usuarios { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string UsuarioCreacion { get; set; }
    public int UsuarioIdCreacion { get; set; }
    public string FotoConversacion { get; set; }
    public List<MensajeModel> Mensajes { get; set; }
    public int Estatus { get; set; }
  }
}
