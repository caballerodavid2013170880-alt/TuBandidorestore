using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public interface ITarifaService
  {
    public List<RutaTipoTarifaModel> ObtenerRutas(int empresaId);
    //public List<EmpresaModel> ObtenerEmpresas(int rutaId);
    public List<TipoTarifaModel> ObtenerTipoTarifa();
    public EmpresaTarifaModel ObtenerPrecioTarifa(EmpresaTarifaModel model);
    Task<EmpresaTarifaModel> ActualizaPrecioTarifa(EmpresaTarifaModel model);
  }
}
