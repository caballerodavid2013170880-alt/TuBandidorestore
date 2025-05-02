using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Service.LogEntidades
{
  public class LoggingInterceptor : SaveChangesInterceptor
  {
    private readonly SuvanDbContext context;

    public LoggingInterceptor(SuvanDbContext context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
      // Acciones antes de guardar cambios, por ejemplo, registrar en la bitácora
      LogChanges(eventData.Context!.ChangeTracker.Entries());

      return base.SavingChanges(eventData, result);
    }

    private void LogChanges(IEnumerable<EntityEntry> entries)
    {
      foreach (var entry in entries)
      {

        var oldValue = entry.OriginalValues.ToObject();
        var newValue = entry.CurrentValues.ToObject();

        // Lógica para determinar si esta entidad debe registrarse en el log
        if (ShouldLogEntity(entry.Entity))
        {
          var logEntry = new Logtransaccionesentidade
          {
            Nombreentidad = entry.Metadata.Name,
            Identidad = entry.OriginalValues[entry.Metadata.FindPrimaryKey()!.Properties[0]]!.ToString(),
            Usuario = ""/* Obtener el nombre del usuario actual */,
            Fecha = DateTime.Now.ToString(),

          };
          if (entry.State == EntityState.Modified)
          {

            logEntry.Accion = "Update";
            logEntry.Valoranterior = JsonConvert.SerializeObject(oldValue);
            logEntry.Valornuevo = JsonConvert.SerializeObject(newValue);
          }
          else if (entry.State == EntityState.Added)
          {
            logEntry.Accion = "Add";
            logEntry.Valornuevo = JsonConvert.SerializeObject(newValue);
          }
          else if (entry.State == EntityState.Deleted)
          {
            logEntry.Accion = "Delete";
            logEntry.Valoranterior = JsonConvert.SerializeObject(oldValue);
          }


          // Guardar logEntry en la base de datos
          context.Logtransaccionesentidades.Add(logEntry);

        }
      }
      context.SaveChanges();
    }


    private static bool ShouldLogEntity(object entity)
    {
      /// Lista de nombres de entidades permitidas para el registro en la bitácora
      var allowedEntities = new List<string> { "variableglobal", "variableempresa", /* mas entidades */ };

      // Obtén el nombre de la entidad actual
      var entityName = entity.GetType().Name;

      // Verifica si la entidad está en la lista blanca
      return allowedEntities.Contains(entityName);
    }
  }

}
