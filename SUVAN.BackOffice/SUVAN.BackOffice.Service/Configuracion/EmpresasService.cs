using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class EmpresasService : IEmpresasService
  {
    private readonly SuvanDbContext context;

    public EmpresasService(SuvanDbContext context)
    {
      this.context = context;
    }


    /// <summary>
    /// Obtiene el listado de empresas desde la base de datos.
    /// </summary>
    /// <returns>Lista de empresas.</returns>
    public async Task<List<Empresa>> GetEmpresas()
    {
      var empresas = await context.Empresas.ToListAsync();

      return empresas!;
    }

    /// <summary>
    /// Obtiene el ViewModel para la empresa específica.
    /// </summary>
    /// <param name="id">Identificador de la empresa.</param>
    /// <returns>ViewModel para la empresa específica.</returns>
    public async Task<AgregarEmpresaViewModel> GetEmpresasViewModel(int id)
    {
      AgregarEmpresaViewModel vRet = new AgregarEmpresaViewModel();
      var empresa = await context.Empresas.FirstOrDefaultAsync(x => x.Idempresa == id);

      if (empresa == null)
        return vRet;
      else
      {
        var dfE = await context.Datosfacturacionemisors.FirstOrDefaultAsync(x => x.EmpresaIdempresa == id);
        vRet = new AgregarEmpresaViewModel
        {
          EmpresaId = empresa.Idempresa,
          Nombre = empresa.Nombre!,
          NombreCorto = empresa.Nombrecorto!,
          Rfc = empresa.Rfc!,
          Activo = empresa.Activo == 1,
          idRegimenFiscal = dfE == null ? 0 : int.Parse(dfE.Regimenfiscal ?? "0"),
          CP = dfE == null ? "" : dfE.LugarexpedicionCp!,
          Folio = dfE == null ? 0 : dfE.Folio ?? 0,
          Serie = dfE == null ? "" : dfE.Serie!
        };
      }

      return vRet;

    }
    /// <summary>
    /// Agrega o actualiza una empresa en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel con los datos de la empresa.</param>
    /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> AgregarEmpresa(AgregarEmpresaViewModel model)
    {
      Empresa empresa;
      Datosfacturacionemisor dfEmisor;

      if (model.EmpresaId > 0)
      {
        empresa = await context.Empresas.FirstOrDefaultAsync(x => x.Idempresa == model.EmpresaId);
        dfEmisor = await context.Datosfacturacionemisors.FirstOrDefaultAsync(x => x.EmpresaIdempresa == empresa.Idempresa);

        if (empresa == null)
          throw new Exception("No se encontro la empresa");

        if (dfEmisor == null)
          dfEmisor = new Datosfacturacionemisor();
      }
      else
      {
        empresa = new Empresa();
        dfEmisor = new Datosfacturacionemisor();
      }


      // validate if existe one empresa with the same name and rfc and is not the same empresa
      var empresaExistente = await context.Empresas.FirstOrDefaultAsync(x =>
      x.Nombre!.ToLower() == model.Nombre!.ToLower()
      && x.Idempresa != model.EmpresaId);

      if (empresaExistente is not null)
        throw new Exception("Ya existe una empresa con el mismo nombre ");

      var empresaExistenteRFC = await context.Empresas.FirstOrDefaultAsync(x =>
     x.Rfc!.ToLower() == model.Rfc!.ToLower()
    && x.Idempresa != model.EmpresaId);

      if (empresaExistenteRFC is not null)
        throw new Exception("Ya existe una empresa con el mismo RFC");

      empresa.Nombre = model.Nombre;
      empresa.Nombrecorto = model.NombreCorto;
      empresa.Rfc = model.Rfc;
      //empresa.Idregimenfiscal = model.idRegimenFiscal;
      empresa.Activo = (ulong?)(model.Activo ? 1 : 0);
      empresa.Fecharegistro = DateTime.Now;

      dfEmisor.Regimenfiscal = model.idRegimenFiscal.ToString();
      dfEmisor.Serie = model.Serie;
      dfEmisor.Folio = model.Folio;
      dfEmisor.LugarexpedicionCp = model.CP.Trim();
      dfEmisor.Fecharegistro = DateTime.Now;


      if (model.EmpresaId > 0)
      {
        context.Empresas.Entry(empresa);

        if (dfEmisor.EmpresaIdempresa == 0)
        {
          dfEmisor.EmpresaIdempresa = model.EmpresaId;
          context.Datosfacturacionemisors.Add(dfEmisor);
        }
        else
          context.Datosfacturacionemisors.Entry(dfEmisor);

        await context.SaveChangesAsync();
      }
      else
      {
        context.Empresas.Add(empresa);
        await context.SaveChangesAsync();

        dfEmisor.EmpresaIdempresa = empresa.Idempresa;
        context.Datosfacturacionemisors.Add(dfEmisor);

        await context.SaveChangesAsync();
      }
      return true;
    }



    public List<TipoRegimenFiscalModel> ObtenerTipoRegimen()
    {
      var resul = (from o in context.Regimenfiscalreceptors
                   select new TipoRegimenFiscalModel()
                   {
                     idRegimenFiscal = o.Idregimenfiscalreceptor,
                     clave = o.Clave,
                     descripcion = o.Descripcion,
                   }).ToList();
      return resul;
    }

  }
}
