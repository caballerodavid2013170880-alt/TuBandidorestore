using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.AtributosModels
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class UnicaHoraInicioAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      var corridaViewModel = (AgregarCorridaViewModel)value;

      var horasInicioDuplicadas = corridaViewModel.Corridas
          .GroupBy(c => c.Inicio)
          .Any(grp => grp.Count() > 1);

      if (horasInicioDuplicadas)
      {
        return new ValidationResult(ErrorMessage ?? "No pueden existir dos registros con la misma hora de inicio.");
      }

      return ValidationResult.Success!;
    }
  }


  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class AnyDiasActivoAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      var dias = value as List<DiasViewModel>;

      if (dias == null || !dias.Any(d => d.Activo))
      {
        return new ValidationResult(ErrorMessage ?? "Al menos un día debe estar activo.");
      }

      return ValidationResult.Success!;
    }
  }

  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class HoraFinMayorQueInicioAttribute : ValidationAttribute
  {
    private readonly string _horaInicioPropertyName;

    public HoraFinMayorQueInicioAttribute(string horaInicioPropertyName)
    {
      _horaInicioPropertyName = horaInicioPropertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      var horaFinProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
      var horaFinValue = (TimeOnly)value;

      var horaInicioProperty = validationContext.ObjectType.GetProperty(_horaInicioPropertyName);
      var horaInicioValue = (TimeOnly)horaInicioProperty!.GetValue(validationContext.ObjectInstance)!;

      if (horaFinValue <= horaInicioValue)
      {
        return new ValidationResult(ErrorMessage ?? "La hora de fin debe ser mayor que la hora de inicio.");
      }

      return ValidationResult.Success!;
    }
  }
}
