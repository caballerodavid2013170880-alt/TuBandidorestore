using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Mensajeria
{
    public class ConversacionConexionModel
    {
        public int ConversacionId { get; set; }
        public int TipoConversacion { get; set; }
        public string NombreConversacion { get; set; }
        public string ConexionId { get; set; }
    }
}
