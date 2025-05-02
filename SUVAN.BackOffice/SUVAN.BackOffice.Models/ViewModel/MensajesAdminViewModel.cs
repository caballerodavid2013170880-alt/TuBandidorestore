using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
  public class MensajesAdminViewModel
  {
    public int MensajeId { get; set; }
    public DateTime FechaMensaje { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string Comentario { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
  }
}
