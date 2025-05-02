using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Contenido
{
    public class ContenidoModel
    {
        public int? Id { get; set; }
        public string? Titulo { get; set; }
    }

    public class ContenidoResponse : ContenidoModel
    {
        public int? TipoContenido { get; set; }
        public string? Html { get; set; }
        public string? Imagen { get; set; }
        public int? Orden { get; set; }
    }

    public class ContenidoGeneral : ContenidoModel
    {
        public bool Coleccion { get; set; }

        public int Tipo { get; set; }

        public string? Imagen { get; set; }
    }

    public class ContenidoMembresiaResponse 
    {
        public List<ContenidoResponse> contenidoresponse { get; set; }

        public List<infoVariables> InfoVariables { get; set; }
    }


    public class infoVariables
    {

        public string? codigo { get; set; }

        public string? valor { get; set; }
    }
}
