using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Enums
{
  public enum EnumTipoDescuento : int
  {
    Cantidad = 1,
    Porcentaje = 2
  }

  public enum EnumTipoCodigo : int
  {
    General = 1,
    Exclisivo = 2
  }

  public enum EnumTipoPolitica : int
  {
    Cancelacion = 1,
    Compensacion = 2
  }

  public enum EnumTipoPoliticaCancelacion : int
  {
    Cliente = 1,
    Empresa = 2
  }

  public enum EnumTipoTiempoPoliticaCancelacion : int
  {
    Dias = 1,
    Horas = 2
  }
}
