using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Logistica
{
  public class RegionesService : IRegionService
  {
    private readonly SuvanDbContext context;

    public RegionesService(SuvanDbContext context)
    {
      this.context = context;
    }
        //260526 POost 2do Scaffold
        /// <summary>
        /// Obtiene el listado de regiones de la empresa indicada,
        /// incluyendo la navegación a <see cref="Empresa"/> (<c>IdEmpresaNavigation</c>)
        /// para mostrar el nombre de empresa en la tabla de la vista <c>Regiones.cshtml</c>.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.
        /// Actúa como filtro de seguridad sobre la consulta.
        /// </param>
        /// <returns>
        /// Lista de <see cref="Region"/> con <c>IdEmpresaNavigation</c> cargada
        /// mediante eager loading, ordenadas por nombre de región.
        /// </returns>
        public async Task<List<Region>> GetRegiones(int idEmpresa)
        {
            var regiones = await context.Regions
                .Include(r => r.IdEmpresaNavigation)   // Necesario para mostrar nombre de empresa en la tabla
                .Where(r => r.IdEmpresa == idEmpresa)
                .OrderBy(r => r.NombreRegion)
                .ToListAsync();
            return regiones;
        }

        /// <summary>
        /// Construye el ViewModel para el formulario de alta o edición de una región.
        /// Al editar, verifica que la región pertenezca a la empresa del usuario.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.Asigna al campo IdEmpresa del ViewModel (al agregar).
        /// </param>
        /// <param name="idRegion">
        /// Identificador de la región a editar. Pasar 0 para modo agregar.
        /// </param>
        /// <returns>
        /// <see cref="RegionViewModel"/> con los datos de la región o vacío para nueva.
        /// El campo <c>ActivoBool</c> se inicializa en <c>true</c> al crear.
        /// </returns>
        /// <exception cref="Exception">
        /// Si <paramref name="idRegion"/> es mayor a <c>0</c> y la región no existe
        /// o no pertenece a la empresa del usuario.
        /// </exception>
        public async Task<RegionViewModel> GetRegionViewModel(int idEmpresa, int idRegion)
        {
            // Inicializar ViewModel con empresa del usuario y activo por defecto
            var vRet = new RegionViewModel
            {
                IdEmpresa = idEmpresa,
                ActivoBool = true   // Activo por defecto al crear
            };
            // Edición: carga datos de la región existente
            if (idRegion > 0)
            {
                // Validación de seguridad: la región debe pertenecer a la empresa del usuario
                var region = await context.Regions
                    .FirstOrDefaultAsync(r => r.IdRegion == idRegion && r.IdEmpresa == idEmpresa);
                if (region == null)
                    throw new Exception("La región no pertenece a su empresa o no existe.");
                vRet.IdRegion = region.IdRegion;
                vRet.IdEmpresa = region.IdEmpresa;
                vRet.NombreRegion = region.NombreRegion;
                vRet.Activo = region.Activo ?? 0;
            }
            return vRet;
        }

        //260526
        /// <summary>
        /// Agrega o actualiza una región en la base de datos.
        /// Validaciones:
        /// <list type="bullet">
        ///   <item>La empresa del ViewModel coincide con la empresa del usuario autenticado.</item>
        ///   <item>Al Editar, la región existe y pertenece a la empresa del usuario.</item>
        ///   <item>No existe una región con el mismo nombre dentro de la misma empresa (excluyendo el registro actual en edición).</item>
        /// </list>
        /// </summary>
        /// <param name="model">ViewModel con los datos capturados en el formulario.</param>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado. Validacion seguridad y sobrescribir el campo empresa en la entidad.
        /// </param>
        /// <returns><c>true</c> si la operación fue exitosa.</returns>
        /// <exception cref="Exception">
        /// Si alguna validación de seguridad o de negocio falla.
        /// </exception>
        public async Task<bool> AgregarRegion(RegionViewModel model, int idEmpresa)
        {
            // Validación de seguridad: la empresa del formulario debe coincidir con la del usuario
            if (model.IdEmpresa != idEmpresa)
                throw new Exception("No tiene permisos para operar sobre esta empresa.");
            Region region;
            if (model.IdRegion > 0)
            {
                // Edición — valida que la región pertenece a la empresa del usuario
                region = await context.Regions
                    .FirstOrDefaultAsync(r => r.IdRegion == model.IdRegion && r.IdEmpresa == idEmpresa);
                if (region == null)
                    throw new Exception("La región no pertenece a su empresa o no existe.");
            }
            else
            {
                // 090726 Modo alta — crear nueva instancia. La base de datos asignará IdRegion automáticamente (ValueGeneratedOnAdd).
                region = new Region
                {
                    IdEmpresa = idEmpresa
                };
            }
            // Valida existencia de regiones con el mismo nombre dentro de la misma empresa
            // En edición se excluye el registro actual para permitir guardar sin cambiar el nombre
            bool nombreDuplicado = await context.Regions
                .AnyAsync(r =>
                    r.NombreRegion!.Trim().ToLower() == model.NombreRegion!.Trim().ToLower() &&
                    r.IdEmpresa == idEmpresa &&
                    r.IdRegion != model.IdRegion);
            if (nombreDuplicado)
                throw new Exception("Ya existe una Región con el mismo nombre en esta Empresa.");
            // Asignar valores a la entidad
            region.NombreRegion = model.NombreRegion;
            region.Activo = model.Activo;
            if (model.IdRegion > 0)
            {
                // Actualizar: la entidad está rastreada por el contexto, solo guardar cambios
                await context.SaveChangesAsync();
            }
            else
            {
                // Insertar nuevo registro
                context.Regions.Add(region);
                await context.SaveChangesAsync();
            }
            return true;
        }



        //NO se usa
        /*
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
        */

  }
}
