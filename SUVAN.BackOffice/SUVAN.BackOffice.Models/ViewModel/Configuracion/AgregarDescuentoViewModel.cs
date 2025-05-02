using SUVAN.BackOffice.Models.ViewModel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
  public class AgregarDescuentoViewModel
  {
    public int DescuentoId { get; set; }

    [Required(ErrorMessage = "El código es requerido")]
    public string Codigo { get; set; } = null!;

    [Required(ErrorMessage = "La Vigencia es requerida")]
    public DateTime Desde { get; set; }

    [Required(ErrorMessage = "La Vigencia es requerida")]
    public DateTime Hasta { get; set; }

    [Required(ErrorMessage = "La Vigencia es requerida")]
    public string Vigencia { get; set; } = string.Empty;

    [Required(ErrorMessage = "El descuento es requerido")]
    public decimal Cantidad { get; set; } = 0;

    public bool Activo { get; set; } = true;

    public int TipoDescuento { get; set; }

    public int TipoCodigo { get; set; }


    [ValidarCorreosExclusivos]
    public string? Correos { get; set; }

    public List<string> CorreosExclusivos { get; set; } = new List<string>();

  }

  public class ValidarCorreosExclusivosAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      // Permitir un valor nulo o una lista vacía
      if (value == null || !(value is string correos) || string.IsNullOrWhiteSpace(correos))
      {
        return ValidationResult.Success!;
      }

      var correosArray = correos.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

      // Verificar que los correos sean válidos
      if (correosArray.Any(correo => !EsCorreoValido(correo.Trim())))
      {
        return new ValidationResult("Ingrese correos electrónicos válidos, uno por línea.");
      }

      return ValidationResult.Success!;
    }

    private bool EsCorreoValido(string correo)
    {
      try
      {
        var mailAddress = new System.Net.Mail.MailAddress(correo);
        return mailAddress.Address == correo;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
