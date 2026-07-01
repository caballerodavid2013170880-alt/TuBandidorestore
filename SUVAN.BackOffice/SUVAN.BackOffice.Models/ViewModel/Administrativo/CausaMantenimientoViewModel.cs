using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class CausaMantenimientoViewModel
    {
        public int IdCausamantenimiento { get; set; }

        public string? Descripcion { get; set; } = null!;

        public string IdDescripcion => $"{IdCausamantenimiento} - {Descripcion}";

        public List<TipoServicioViewModel> TipoServicioView { get; set; } = new();

        public class TipoServicioViewModel
        {
            public int IdTiposervicio { get; set; }

            public string ServicioNombre { get; set; } = null!;

            public string IdNombreS => $"{IdTiposervicio} - {ServicioNombre}";
        }
        
    }
}