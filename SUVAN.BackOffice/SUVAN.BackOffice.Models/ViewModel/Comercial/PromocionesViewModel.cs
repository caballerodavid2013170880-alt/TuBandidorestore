using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Comercial
{
  public class GeneraPromocionViewModel
  {
    public PromocionesViewModel Promocion { get; set; }
    public List<TipoPromocionViewModel> TipoPromocion { get; set; }
    public List<TipoDescuentoViewModel> TipoDescuento { get; set; }
    public List<RutaEmpresaViewModel> RutaCorridas { get; set; }
    public GeneraPromocionViewModel()
    {
      this.Promocion = new PromocionesViewModel();
      this.TipoPromocion = new List<TipoPromocionViewModel>();
      this.TipoDescuento = new List<TipoDescuentoViewModel>();
      this.RutaCorridas = new List<RutaEmpresaViewModel>();
    }

  }
  public class TipoPromocionViewModel
  {
    public int TipoPromocionId { get; set; }
    public string? NombrePromocion { get; set; }
  }
  public class TipoDescuentoViewModel
  {
    public int TipoDescuendoId { get; set; }
    public string? NombreDescuento { get; set; }
  }
  public class UsuarioEmpresaModel
  {
    public int EmpresaId { get; set; }
    public string? NombreEmpresa { get; set; }
  }
  public class RutaEmpresaViewModel
  {
    public int RutaId { get; set; }
    public string? NombreRuta { get; set; }
    public List<CorridaViewModel> Corridas { get; set; }
    public RutaEmpresaViewModel()
    {
      this.Corridas = new List<CorridaViewModel>();

    }

  }
  public class CorridaViewModel
  {
    public int CorridaId { get; set; }
    public TimeOnly? HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
  }
  public class HorarioViewModel
  {
    public TimeOnly? HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
  }
  public class PromocionesViewModel
  {
    public int PromocionId { get; set; }
    public string? Nombre { get; set; }
    public int TipoDescuentoId { get; set; }
    public int TipoPromocionId { get; set; }
    public string? NombreEmpresa { get; set; }
    public int EmpresaId { get; set; }
    public string? DescripcionDescuento { get; set; }
    public decimal? MontoDescuento { get; set; }
    public string? AplicaPara { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int[] RutasEmpresa { get; set; }
    public int[] CorridasRutas { get; set; }
    public HorarioViewModel[] Horarios { get; set; }
    public PromocionesViewModel()
    {
      this.RutasEmpresa = Array.Empty<int>();
      this.CorridasRutas = Array.Empty<int>();
      this.Horarios = Array.Empty<HorarioViewModel>();
    }
  }

  public class GeneraReciboViewModel
  {
    public int operadorId { get; set; }
    public string fechaInicio { get; set; } = string.Empty;
    public string fechaFin { get; set; } = string.Empty;
  }
}
