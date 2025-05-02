using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  public class AgregarConductorViewModel
  {
    public int ConductorId { get; set; }

    [Required(ErrorMessage = "El Nombre es requerido")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El RFC es requerido")]
    public string RFC { get; set; } = null!;
    public bool Activo { get; set; } = true;

    public IFormFile? Imagen { get; set; } = null;

    public string? Imagen64 { get; set; } = null;
    public string Direccion { get; set; } = string.Empty;

    public string Curp { get; set; } = string.Empty;

    public string Ine { get; set; } = string.Empty;

    public string TipoSangre { get; set; } = string.Empty;
    public string NumeroLicencia { get; set; } = string.Empty;

    public string TipoLicencia { get; set; } = string.Empty;
    public int? RegimenFiscalId { get; set; }

    public string Cif { get; set; } = string.Empty;

    public string NombreContacto { get; set; } = string.Empty;
    public string TelefonoContacto { get; set; } = string.Empty;

    public decimal? Comisionfija { get; set; }

    public decimal? ComisionvariableKm { get; set; }

    public decimal? ComisionvariableIngresos { get; set; }
    public string Correo { get; set; } = null!;
    public List<RegimenFiscalViewModel> RegimenFiscal { get; set; } = new List<RegimenFiscalViewModel>();
  }

  public class RegimenFiscalViewModel
  {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
  }

  public class ReporteConductorViewModel
  {
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public string Empresa { get; set; } = null!;
    public string Unidades { get; set; } = string.Empty;

  }
}
