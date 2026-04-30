using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class RegionesService : IRegionService
  {
    private readonly SuvanDbContext context;

    public RegionesService(SuvanDbContext context)
    {
      this.context = context;
    }


        /// <summary>
        /// Obtiene el listado de empresas desde la base de datos.
        /// </summary>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <returns>Lista de empresas.</returns>
        public async Task<List<Region>> GetRegiones(int id_empresa)
        {
            var deptos = await context.Regions.Where(x => x.IdEmpresa == id_empresa).ToListAsync();

            return deptos!;
        }

        /// <summary>
        /// Obtiene el ViewModel para la región específica.
        /// </summary>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <param name="id_region">Identificador de la región.</param>
        /// <returns>ViewModel para la región específica.</returns>
        public async Task<RegionViewModel> GetRegionViewModel(int id_empresa, int id_region)
        {
            RegionViewModel vRet = new RegionViewModel();
            var region = await context.Regions.FirstOrDefaultAsync(x => x.IdEmpresa == id_empresa && x.IdRegion == id_region);

            if (region == null)
                return vRet;
            else
            {
                vRet = new RegionViewModel
                {
                    id_empresa = region.IdEmpresa,
                    id_region = region.IdRegion,
                    nombre = region.Nombre
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza una region en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la region.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarRegion(RegionViewModel model)
        {
            Region region;

            if (model.id_region > 0)
            {
                region = await context.Regions.FirstOrDefaultAsync(x => x.IdEmpresa == model.id_empresa && x.IdRegion == model.id_region);

                if (region == null)
                    throw new Exception("No se encontro la region");
            }
            else
            {
                region = new Region();
                region.IdEmpresa = model.id_empresa;
                //var vLastRow = await context.Regions.OrderBy(x => x.IdEmpresa).LastOrDefaultAsync(x => x.IdEmpresa == model.id_empresa);
                //IdRegion desc para valor más alto
                var vLastRow = await context.Regions.Where(x => x.IdEmpresa == model.id_empresa).OrderByDescending(x => x.IdRegion).FirstOrDefaultAsync();
                region.IdRegion = (short)((vLastRow != null ? vLastRow.IdRegion : 0) + 1);
            }


            // validate if exist one region with the same name in the same empresa
            var regionExistente = await context.Regions.FirstOrDefaultAsync(x => x.Nombre!.ToLower() == model.nombre!.ToLower()
            && x.IdEmpresa == model.id_empresa //Valida si empresa evitando duplicados
            && x.IdRegion != model.id_region); //Valida region diferente

            if (regionExistente is not null)
                throw new Exception("Ya existe una región con el mismo nombre en la empresa.");

            region.Nombre = model.nombre;

            if (model.id_region > 0)
            {
                context.Regions.Entry(region);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Regions.Add(region);
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
