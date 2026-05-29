using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ObjectUsuarios
{
    public class ObjectConductor
    {
        public int Idconductor { get; set; }

        public string? Email { get; set; }

        public string? Hashpass { get; set; }

        public int? CodigopaisIdcodigopais { get; set; }

        public string? Telefono { get; set; }

        public string? CodigoAuth { get; set; }

        public ulong? Validado { get; set; }

        public ulong? Activo { get; set; }

        public string? Nombre { get; set; }

        public string? Rfc { get; set; }

        public DateTime? CodigoExp { get; set; }
    }
}
