using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Mensajeria.Operador
{
    public class MensajeOperadorModel
    {
        public int conductor_id { get; set; }
        public int conversacion_id { get; set; }
        public string Mensaje { get; set; }
    }
}
