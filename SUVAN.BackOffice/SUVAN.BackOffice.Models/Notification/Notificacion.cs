using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Notification
{
    public class Notificacion
    {
        public string to { get; set; }
        public Notification notification { get; set; }
        public object data { get; set; }
    }


    public class Data
    {
        public string comando { get; set; }
        public int reserva_id { get; set; }
        public int corrida_asignacion_id { get; set; }
    }

    public class DataChat
    {
        public string comando { get; set; }
        public int conversacion_id { get; set; }
    }

    public class Notification
    {
        public string title { get; set; }
        public string body { get; set; }
        public string icon { get; set; }
        public string sound { get; set; }
        public string click_action { get; set; }

    }


}
