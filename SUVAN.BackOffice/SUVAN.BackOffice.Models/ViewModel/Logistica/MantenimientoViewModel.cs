using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class MantenimientoViewModel
    {
        public List<ReporteOrdenViewModel> ReporteOrdenView { get; set; } = new();

        public class ReporteOrdenViewModel
        {
            public string Desde { get; set; } = string.Empty;
            public string Hasta { get; set; } = string.Empty;
        }

    }
}
