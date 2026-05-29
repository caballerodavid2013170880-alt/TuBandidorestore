using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Service.LogEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class VariablesService : IVariablesService
  {
    private readonly SuvanDbContext context;
    private readonly ILogTransaccionesEntidadesService logTransacciones;

    public VariablesService(SuvanDbContext context,
      ILogTransaccionesEntidadesService logTransacciones)
    {
      this.context = context;
      this.logTransacciones = logTransacciones;
    }

    /// <summary>
    /// obtine las variables de la empresa para configurarlas
    /// </summary>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<AgregarVariableViewModel> GetVariablesEmpresa(int empresaId)
    {
      var variablesViewModel = new AgregarVariableViewModel();

      var variablesEmpresa = await context.Variableempresas
        .Include(x => x.VariableIdvariableNavigation)
        .Where(x => x.EmpresaIdempresa == empresaId)
        .ToListAsync();

      if (!variablesEmpresa.Any())
      {
        var variablesConfig = await context.Variables
          .Where(x => x.TipovariableIdtipovariable == (int)EnumTipoVariable.Empresa)
          .ToListAsync();

        foreach (var variable in variablesConfig)
        {
          variablesViewModel.Variables.Add(new VariableViewModel
          {
            VariableId = variable.Idvariable,
            Descripcion = variable.Descripcion!,
            Valor = string.Empty
          });
        }
      }
      else
      {
        variablesViewModel.Variables = variablesEmpresa.Select(x => new VariableViewModel
        {
          VariableId = x.VariableIdvariable,
          Descripcion = x.VariableIdvariableNavigation.Descripcion!,
          Valor = x.Valor!
        }).ToList();
      }

      variablesViewModel.CantidadVariables = variablesViewModel.Variables.Count;

      return variablesViewModel;
    }

    /// <summary>
    /// obtiene las variables globales para configurarlas
    /// </summary>
    /// <returns></returns>
    public async Task<AgregarVariableViewModel> GetVariablesGlobales()
    {
      var variablesViewModel = new AgregarVariableViewModel();

      var variablesConfig = await context.Variables
         .Where(x => x.TipovariableIdtipovariable == (int)EnumTipoVariable.Global)
         .ToListAsync();

      var variablesGlobales = await context.Variableglobals
        .Include(x => x.VariableIdvariableNavigation)
        .ToListAsync();

      foreach (var variable in variablesConfig)
      {
        var global = variablesGlobales.FirstOrDefault(x => x.VariableIdvariable == variable.Idvariable);
        variablesViewModel.Variables.Add(new VariableViewModel
        {
          VariableId = variable.Idvariable,
          Descripcion = variable.Descripcion!,
          Valor = global == null ? string.Empty : global.Valor!,
          Lista = variable.Lista != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<VariableListViewModel>>(variable.Lista!) : null
        });
      }





      //if (!variablesGlobales.Any())
      //{


      //  foreach (var variable in variablesConfig)
      //  {
      //    variablesViewModel.Variables.Add(new VariableViewModel
      //    {
      //      VariableId = variable.Idvariable,
      //      Descripcion = variable.Descripcion!,
      //      Valor = string.Empty
      //    });
      //  }
      //}
      //else
      //{
      //  variablesViewModel.Variables = variablesGlobales.Select(x => new VariableViewModel
      //  {
      //    VariableId = x.VariableIdvariable,
      //    Descripcion = x.VariableIdvariableNavigation.Descripcion!,
      //    Valor = x.Valor!
      //  }).ToList();
      //}

      variablesViewModel.CantidadVariables = variablesViewModel.Variables.Count;

      return variablesViewModel;

    }


    /// <summary>
    /// agrega las variables de la empresa o globales
    /// </summary>
    /// <param name="model"></param>
    /// <param name="empresaId"></param>
    /// <param name="tipo"></param>
    /// <returns></returns>
    public async Task<bool> AgregarVariables(AgregarVariableViewModel model, int empresaId, EnumTipoVariable tipo, string usuario)
    {


      if (tipo == EnumTipoVariable.Empresa)
      {
        var variablesEmpresa = await context.Variableempresas
          .Where(x => x.EmpresaIdempresa == empresaId)
          .ExecuteDeleteAsync();
        await context.SaveChangesAsync();
        foreach (var variable in model.Variables)
        {
          context.Variableempresas.Add(new Variableempresa
          {
            EmpresaIdempresa = empresaId,
            VariableIdvariable = variable.VariableId,
            Valor = variable.Valor
          });
        }
        await context.SaveChangesAsync();

        await GuardarBitacora(empresaId, tipo, usuario, false);
      }
      else
      {
        var variablesGlobales = await context.Variableglobals
          .ExecuteDeleteAsync();
        await context.SaveChangesAsync();
        foreach (var variable in model.Variables)
        {
          await context.Variableglobals.AddAsync(new Variableglobal
          {
            VariableIdvariable = variable.VariableId,
            Valor = variable.Valor
          });
        }

        await context.SaveChangesAsync();
        await GuardarBitacora(empresaId, tipo, usuario, false);

      }


      return true;
    }

    private async Task GuardarBitacora(int empresaId, EnumTipoVariable tipo, string usuario, bool isUpdate)
    {
      if (tipo == EnumTipoVariable.Empresa)
      {
        var listVariables = await context.Variableempresas
          .Where(x => x.EmpresaIdempresa == empresaId)
          .ToListAsync();

        foreach (var variable in listVariables)
        {
          await logTransacciones.AddBitacora(new Logtransaccionesentidade
          {
            Nombreentidad = "VariableEmpresa",
            Identidad = variable.VariableIdvariable.ToString()!,
            Usuario = $"{usuario} ID Empresa {empresaId}",
            Fecha = DateTime.Now.ToString(),
            Accion = isUpdate ? "Update" : "Create",
            Valoranterior = variable.Valor,
            Valornuevo = variable.Valor,

          });
        }
      }
      else
      {
        var variablesGlobales = await context.Variableglobals.ToListAsync();

        foreach (var variable in variablesGlobales)
        {
          await logTransacciones.AddBitacora(new Logtransaccionesentidade
          {
            Nombreentidad = "VariableGlobal",
            Identidad = variable.VariableIdvariable.ToString()!,
            Usuario = usuario,
            Fecha = DateTime.Now.ToString(),
            Accion = isUpdate ? "Update" : "Create",
            Valoranterior = variable.Valor,
            Valornuevo = variable.Valor,
          });
        }

      }
    }
  }
}
