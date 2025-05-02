using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
    public class EstablecerTarifaViewModel
    {
        public List<RutaTipoTarifaModel> Rutas { get; set; }
        //public List<EmpresaModel> Empresas { get; set; }
        public List<TipoTarifaModel> TipoTarifas { get; set; }

        public RutaTipoTarifaModel Ruta { get; set; } = new RutaTipoTarifaModel();

        public int RutaId { get; set; }
        public int EmpresaId { get; set; }
        public int TipoTarifaId { get; set; }

        public EstablecerTarifaViewModel()
        {
            Rutas = new List<RutaTipoTarifaModel>();
            //Empresas = new List<EmpresaModel>();
            TipoTarifas = new List<TipoTarifaModel>();

        }
    }
}
