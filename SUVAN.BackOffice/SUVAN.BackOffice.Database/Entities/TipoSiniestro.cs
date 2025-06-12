using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Database.Entities
{
    internal class TipoSiniestro
    {
        [Key]
        public int Id_tipo_siniestro { get; set; }

        public int Id_causa_siniestro { get; set; }
        public string? Descripcion { get; set; }

    }
}
