using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class TipoEjeViewModel
    {
        public int IdTipoEje { get; set; }

        public string Descripcion { get; set; } = null!;

        public string IdDescripcion => $"{IdTipoEje} - {Descripcion}";
    }
}
