using SUVAN.BackOffice.Models.ViewModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Ingresos
{
  public class ReporteIngresosViewModel
  {
    public string? Desde { get; set; } = string.Empty;
    public string? Hasta { get; set; } = string.Empty;

    public string? desdeview { get; set; }
    public string? hastaview { get; set; }
    public int? EmpresaId { get; set; }
    public int? MetodoPagoId { get; set; }

    public List<CatalogoIngresosViewModel> Empresas { get; set; } = new();
    public List<CatalogoIngresosViewModel> MetodoPago { get; set; } = new();
    public EnumPeriodoReporte Periodo { get; set; }

    public List<ReporteIngresosDetalleViewModel> Detalle { get; set; } = new();

    public string Total { get; set; } = string.Empty;

  }

  public class CatalogoIngresosViewModel
  {
    public int Id { get; set; }
    public string Descripcion { get; set; } = null!;
  }

  public class ReporteIngresosDetalleViewModel
  {
    public DateTime Fecha { get; set; }
    public string Empresa { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = string.Empty;
    public string TipoPago { get; set; } = string.Empty;
    public decimal Cantidad { get; set; } = 0;
  }
}
