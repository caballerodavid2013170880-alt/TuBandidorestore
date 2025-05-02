using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.ObjectParentResponse;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SUVAN.BackOffice.Models.Errores
{
    public class ErroresResponse: ObjectParentResponse.ObjectParentResponse
    {

        private Objeto _objeto;

        public Objeto objeto
        {
            get { return _objeto; }
            set { _objeto = value; }
        }

        public class Objeto
        {
            [DataMember(Order = 0)]
            public string Codigo { get; set; }

            [DataMember(Order = 1)]
            public string Nombre { get; set; }

            [DataMember(Order = 2)]
            public string Mensaje { get; set; }
        }

    }
}
