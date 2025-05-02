using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Mensajeria
{
    public class BandejaConversacionesModel
    {
        public int? ConversacionId { get; set; }
        public string NombreConversacion { get; set; }
        public int TipoConversacion { get; set; }
        public string Abreviatura { get; set; }
        public string NombreTipo { get; set; }
        public string Estilo { get; set; }
        public DateTime FechaCreacion { get; set;}
        public string DiferenciaTiempo { get; set; }
        public int TotalMensajes { get; set; }
    }
}
