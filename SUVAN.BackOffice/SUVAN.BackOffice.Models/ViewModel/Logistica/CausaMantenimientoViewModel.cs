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

        public int PreventivoIdpreventivo { get; set; }

        public string Nombre { get; set; } = null!;

        public List<PreventivoViewModel> PreventivoView { get; set; } = new();

        public class PreventivoViewModel
        {
            public int Idpreventivo { get; set; }
            public string PreventivoNombre { get; set; } = null!;
            public string IdNombreP => $"{Idpreventivo} - {PreventivoNombre}";
            public List<TipoServicioViewModel> TipoServicio { get; set; } = new();

        }

        public class TipoServicioViewModel
        {
            public int IdTiposervicio { get; set; }

            public string ServicioNombre { get; set; } = null!;

            public string IdNombreS => $"{IdTiposervicio} - {ServicioNombre}";
        }
        public class MarcaViewModel
        {
            public int Id_Marca { get; set; }
            public string descrip { get; set; }
            = null!;
        }
    }
}