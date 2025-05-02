using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Mensajeria
{
    public class MensajeModel
    {
        public int MensajeId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }

    }
  
}
