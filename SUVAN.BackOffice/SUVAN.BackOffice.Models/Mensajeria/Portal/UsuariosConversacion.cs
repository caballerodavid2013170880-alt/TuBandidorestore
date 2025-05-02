using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Mensajeria
{

    public class UsuariosConversacion
    {
        public int UsuarioId { get; set; }
        public string TipoUsuario { get; set; }
        
        public string NombreUsuario { get; set; }
        public string? ImagenPerfil { get; set; }

    }
}
