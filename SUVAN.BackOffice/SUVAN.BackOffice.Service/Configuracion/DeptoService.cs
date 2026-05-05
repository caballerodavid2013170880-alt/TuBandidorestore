using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
namespace SUVAN.BackOffice.Service.Configuracion
{
    public class DeptoService : IDeptoService
    {
        private readonly SuvanDbContext context;

        public DeptoService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene el listado de Departamentos filtrando por los que pertenecen a las regiones de la empresa actual.
        /// </summary>
        public async Task<List<Depto>> GetDeptos(int id_empresa)
        {
            var deptos = await (from d in context.Deptos
                                join r in context.Regions on d.IdRegion equals r.IdRegion
                                where r.IdEmpresa == id_empresa
                                select d).ToListAsync();

            return deptos;
        }

        /// <summary>
        /// Obtiene el ViewModel para editar un departamento específico.
        /// </summary>
        public async Task<DeptoViewModel> GetDeptoViewModel(int id_empresa, short id_region, short id_planta, short id_zona, short id_deposi, short id_depto)
        {
            DeptoViewModel vRet = new DeptoViewModel();

            // Si el id_depto es 0, es un registro nuevo
            if (id_depto == 0) return vRet;

            // Validar que el depto pertenezca a la empresa mediante su región
            var region = await context.Regions.FirstOrDefaultAsync(r => r.IdRegion == id_region && r.IdEmpresa == id_empresa);
            if (region == null) return vRet;

            var depto = await context.Deptos.FirstOrDefaultAsync(x =>
                x.IdRegion == id_region &&
                x.IdPlanta == id_planta &&
                x.IdZona == id_zona &&
                x.IdDeposi == id_deposi &&
                x.IdDepto == id_depto);

            if (depto != null)
            {
                vRet = new DeptoViewModel
                {
                    id_region = depto.IdRegion,
                    id_planta = depto.IdPlanta,
                    id_zona = depto.IdZona,
                    id_deposi = depto.IdDeposi,
                    id_depto = depto.IdDepto,
                    descripcion = depto.Descrip,
                    responsable = depto.Responsable
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza un departamento, simulando validaciones de llaves foráneas.
        /// </summary>
        public async Task<bool> AgregarDepto(DeptoViewModel model, int id_empresa)
        {
            // Validaciones simulando FKs
            var region = await context.Regions.FirstOrDefaultAsync(x => x.IdRegion == model.id_region && x.IdEmpresa == id_empresa);
            if (region == null)
                throw new Exception("La Región seleccionada no existe o no pertenece a su empresa.");

            var planta = await context.Planta.FirstOrDefaultAsync(x => x.IdPlanta == model.id_planta && x.IdRegion == model.id_region);
            if (planta == null)
                throw new Exception("La Planta seleccionada no existe o no pertenece a la región indicada.");

            var zona = await context.Zonas.FirstOrDefaultAsync(x => x.IdZona == (int)model.id_zona && x.IdPlanta == model.id_planta && x.IdRegion == model.id_region);
            if (zona == null)
                throw new Exception("La Zona seleccionada no existe o no pertenece a la planta indicada.");

            var deposito = await context.Depositos.FirstOrDefaultAsync(x => x.IdDeposi == model.id_deposi && x.IdZona == model.id_zona && x.IdPlanta == model.id_planta && x.IdRegion == model.id_region);
            if (deposito == null)
                throw new Exception("El Depósito seleccionado no existe o no pertenece a la zona indicada.");

            Depto depto;

            if (model.id_depto > 0)
            {
                depto = await context.Deptos.FirstOrDefaultAsync(x =>
                    x.IdRegion == model.id_region &&
                    x.IdPlanta == model.id_planta &&
                    x.IdZona == model.id_zona &&
                    x.IdDeposi == model.id_deposi &&
                    x.IdDepto == model.id_depto);

                if (depto == null)
                    throw new Exception("No se encontró el departamento para actualizar.");
            }
            else
            {
                depto = new Depto
                {
                    IdRegion = model.id_region,
                    IdPlanta = model.id_planta,
                    IdZona = model.id_zona,
                    IdDeposi = model.id_deposi
                };

                // Autoincremental basado en los padres
                var vLastRow = await context.Deptos
                    .Where(x => x.IdRegion == model.id_region && x.IdPlanta == model.id_planta && x.IdZona == model.id_zona && x.IdDeposi == model.id_deposi)
                    .OrderByDescending(x => x.IdDepto)
                    .FirstOrDefaultAsync();

                depto.IdDepto = (short)((vLastRow != null ? vLastRow.IdDepto : 0) + 1);
            }

            // Validar duplicidad de descripción en la misma jerarquía
            var deptoExistente = await context.Deptos.FirstOrDefaultAsync(x =>
                x.Descrip.ToLower() == model.descripcion.ToLower() &&
                x.IdRegion == model.id_region &&
                x.IdPlanta == model.id_planta &&
                x.IdZona == model.id_zona &&
                x.IdDeposi == model.id_deposi &&
                x.IdDepto != model.id_depto);

            if (deptoExistente != null)
                throw new Exception("Ya existe un departamento con el mismo nombre en este depósito.");

            depto.Descrip = model.descripcion;
            depto.Responsable = model.responsable;

            if (model.id_depto > 0)
            {
                context.Deptos.Entry(depto);
            }
            else
            {
                context.Deptos.Add(depto);
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Zona>> GetZonas(int id_empresa)
        {
            return await context.Zonas.Where(x => x.IdEmpresa == id_empresa).ToListAsync();
        }

        public async Task<List<Deposito>> GetDepositos(int id_empresa)
        {
            return await (from d in context.Depositos
                          join r in context.Regions on d.IdRegion equals r.IdRegion
                          where r.IdEmpresa == id_empresa
                          select d).ToListAsync();
        }
    }
}
