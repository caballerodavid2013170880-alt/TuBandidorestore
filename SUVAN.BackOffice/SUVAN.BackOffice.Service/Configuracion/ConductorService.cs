using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class ConductorService : IConductorService
  {
    private readonly SuvanDbContext context;

    public ConductorService(SuvanDbContext context)
    {
      this.context = context;
    }


    /// <summary>
    /// Obtiene todos los conductores desde la base de datos.
    /// </summary>
    /// <returns>Lista de conductores.</returns>
    public async Task<List<Database.Entities.Conductor>> GetConductores(int empresaId)
    {
      var conductores = await context.Conductors
        .Where(x => x.EmpresaIdempresa == empresaId)
        .ToListAsync();
      return conductores!;
    }

    /// <summary>
    /// Obtiene el ViewModel para el conductor específico.
    /// </summary>
    /// <param name="id">Identificador del conductor.</param>
    /// <returns>ViewModel para el conductor específico.</returns>
    public async Task<AgregarConductorViewModel> GetConductorViewModel(int id, int empresaId)
    {
      AgregarConductorViewModel model = new();

      var conductor = await context.Conductors
        .FirstOrDefaultAsync(x => x.Idconductor == id);

      model.RegimenFiscal = await GetRegimenFiscal();

      if (conductor == null)
        return model;

      model.ConductorId = conductor.Idconductor;
      model.Nombre = conductor.Nombre!;
      model.RFC = conductor.Rfc!;
      model.Imagen64 = conductor.Imagen!;
      model.Activo = conductor.Activo == 1;
      model.Direccion = conductor.Direccion!;
      model.Curp = conductor.Curp!;
      model.Ine = conductor.Ine!;
      model.TipoSangre = conductor.Tiposangre!;
      model.NumeroLicencia = conductor.Numerolicencia!;
      model.TipoLicencia = conductor.Tipolicencia!;
      model.RegimenFiscalId = conductor.Idregimenfiscal;
      model.Cif = conductor.Cif!;
      model.NombreContacto = conductor.Nombrecontacto!;
      model.TelefonoContacto = conductor.Telefonocontacto!;
      model.Comisionfija = conductor.Comisionfija;
      model.ComisionvariableKm = conductor.ComisionvariableKm;
      model.ComisionvariableIngresos = conductor.ComisionvariableIngresos;
      model.Correo = conductor.Email!;

      return model;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<List<RegimenFiscalViewModel>> GetRegimenFiscal()
    {
      return await context.Regimenfiscalreceptors
         .Where(x => x.Fisica == 1)
         .Select(x => new RegimenFiscalViewModel
         {
           Id = x.Idregimenfiscalreceptor,
           Nombre = x.Descripcion!
         })
         .ToListAsync();
    }

    /// <summary>
    /// Agrega o actualiza un conductor en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel con los datos del conductor.</param>
    /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> AgregarConductor(AgregarConductorViewModel model, int empresaId)
    {

      var conductorExistente = await context.Conductors
       .FirstOrDefaultAsync(x =>
       ((x.Nombre == model.Nombre || x.Rfc == model.RFC) && x.EmpresaIdempresa == empresaId)
       && x.Idconductor != model.ConductorId);

      if (conductorExistente != null)
      {
        throw new Exception("Ya existe un conductor con el mismo nombre ó RFC");
      }


      //validate if exist one conductor with the same curp and is not the same conductor
      var conductorExistenteCurp = await context.Conductors
        .FirstOrDefaultAsync(x =>
        x.Curp == model.Curp
        && x.EmpresaIdempresa == empresaId
        && x.Idconductor != model.ConductorId);

      if (conductorExistenteCurp != null)
      {
        throw new Exception("Ya existe un conductor con el mismo CURP");
      }

      Database.Entities.Conductor conductor = new();
      if (model.ConductorId > 0)
      {
        conductor = await context.Conductors
         .FirstOrDefaultAsync(x => x.Idconductor == model.ConductorId);

        if (conductor != null)
        {
          conductor.Nombre = model.Nombre;
          conductor.Rfc = model.RFC;
          if (model.Imagen != null)
          {
            var imageBase64 = model.Imagen.GetBase64();
            if (conductor.Imagen != imageBase64)
            {
              conductor.Imagen = model.Imagen.GetBase64();
            }

          }
          else
          {
            conductor.Imagen = null;
          }
          conductor.Activo = (ulong?)(model.Activo ? 1 : 0);
          conductor.Fecharegistro = DateTime.Now;
          conductor.EmpresaIdempresa = empresaId;
          conductor.Direccion = model.Direccion;
          conductor.Curp = model.Curp;
          conductor.Ine = model.Ine;
          conductor.Tiposangre = model.TipoSangre;
          conductor.Numerolicencia = model.NumeroLicencia;
          conductor.Tipolicencia = model.TipoLicencia;
          conductor.Idregimenfiscal = model.RegimenFiscalId;
          conductor.Cif = model.Cif;
          conductor.Nombrecontacto = model.NombreContacto;
          conductor.Telefonocontacto = model.TelefonoContacto;
          conductor.Comisionfija = model.Comisionfija;
          conductor.ComisionvariableKm = model.ComisionvariableKm;
          conductor.ComisionvariableIngresos = model.ComisionvariableIngresos;
          conductor.Email = model.Correo;

          context.Conductors.Entry(conductor);
        }
      }
      else
      {

        conductor = new Database.Entities.Conductor
        {
          Nombre = model.Nombre,
          Rfc = model.RFC,
          Activo = (ulong?)(model.Activo ? 1 : 0),
          Fecharegistro = DateTime.Now,
          EmpresaIdempresa = empresaId,
          Direccion = model.Direccion,
          Curp = model.Curp,
          Ine = model.Ine,
          Tiposangre = model.TipoSangre,
          Numerolicencia = model.NumeroLicencia,
          Tipolicencia = model.TipoLicencia,
          Idregimenfiscal = model.RegimenFiscalId,
          Cif = model.Cif,
          Nombrecontacto = model.NombreContacto,
          Telefonocontacto = model.TelefonoContacto,
          Comisionfija = model.Comisionfija,
          ComisionvariableKm = model.ComisionvariableKm,
          ComisionvariableIngresos = model.ComisionvariableIngresos,
          Email = model.Correo

        };

        if (model.Imagen != null)
        {
          conductor.Imagen = model.Imagen.GetBase64();
        }

        context.Conductors.Add(conductor);

      }

      await context.SaveChangesAsync();

      return true;
    }


    /// <summary>
    /// obtiene un reporte general de operadores
    /// </summary>
    /// <returns></returns>
    public async Task<List<ReporteConductorViewModel>> ReporteOperadores()
    {
      var today = DateTime.Today.Date;
      var conductores = await (from c in context.Conductors
                               orderby c.EmpresaIdempresa, c.Nombre
                               select new ReporteConductorViewModel
                               {
                                 Nombre = c.Nombre!,
                                 Correo = c.Email!,
                                 Activo = c.Activo == 1,
                                 Empresa = c.EmpresaIdempresaNavigation!.Nombre!,
                                 Unidades = string.Join(", ", c.CorridaAsignacions
                                 .Where(f => f.Fecha!.Value.Date == today)
                                 .Select(x => $"{x.VehiculoIdvehiculoNavigation.TipovehiculoIdtipovehiculoNavigation.Nombre} - {x.VehiculoIdvehiculoNavigation.Placas}")
                                 .Distinct()
                                 .ToList())
                               })
        .ToListAsync();
      //.Where(f => f.Fecha!.Value.Date == DateTime.Today.Date)


      return conductores;

    }

  }
}
