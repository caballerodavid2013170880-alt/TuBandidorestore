using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.CausaMantenimientoViewModel;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class MantenimientoDetalleViewModel
    {
        public int IdMantenimientoDet { get; set; }

        public int IdMantenimiento { get; set; }

        public int Renglon { get; set; }

        public short IdTipoReparacion { get; set; }

        public short? Cantidad { get; set; }

        public string? Descripcion { get; set; } = null!;

        public float? Precio { get; set; }

        public string? TiempoEmpleado { get; set; } = null!;

        public float? ValTall { get; set; }

        public DateTime? FechaProgramada { get; set; }

        public List<TipoReparacionViewModel> TipoReparacionView { get; set; } = new();

        public class TipoReparacionViewModel
        {
            public short IdTipoReparacion { get; set; }

            public string? Descripcion { get; set; }

            public short? Grupo { get; set; }

            public float? Valor { get; set; }
        }

    }
}
