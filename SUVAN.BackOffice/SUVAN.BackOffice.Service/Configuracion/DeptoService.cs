using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
    /// <summary>
    /// Servicio para la gestión de Departamentos.
    /// Implementa la lógica de negocio y mantiene la integridad referencial manual 
    /// para la jerarquía Región -> Planta -> Zona -> Depósito -> Depto.
    /// </summary>
    public class DeptoService : IDeptoService
    {
        private readonly SuvanDbContext context;

        public DeptoService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene el listado de departamentos filtrado por la jerarquía geografica.
        /// </summary>
        /// <param name="id_region">ID de la Región</param>
        /// <param name="id_planta">ID de la Planta</param>
        /// <param name="id_zona">ID de la Zona</param>
        /// <param name="id_deposi">ID del Depósito</param>
        /// <returns>Lista de Departamentos</returns>
        public async Task<List<Depto>> GetDeptos(short id_region, short id_planta, short id_zona, short id_deposi)
        {
            var deptos = await context.Deptos
                .Where(x => x.IdRegion == id_region && x.IdPlanta == id_planta && x.IdZona == id_zona && x.IdDeposi == id_deposi)
                .ToListAsync();
            return deptos!;
        }

        /// <summary>
        /// Obtiene la información de un departamento y su jerarquia.
        /// </summary>
        /// <param name="id_region">ID de la Región</param>
        /// <param name="id_planta">ID de la Planta</param>
        /// <param name="id_zona">ID de la Zona</param>
        /// <param name="id_deposi">ID del Depósito</param>
        /// <param name="id_depto">ID del Departamento a buscar</param>
        /// <returns>ViewModel con los detalles</returns>
        public async Task<DeptoViewModel> GetDeptoViewModel(short id_region, short id_planta, short id_zona, short id_deposi, short id_depto)
        {
            DeptoViewModel vRet = new DeptoViewModel();
            var depto = await context.Deptos.FirstOrDefaultAsync(x =>
                x.IdRegion == id_region &&
                x.IdPlanta == id_planta &&
                x.IdZona == id_zona &&
                x.IdDeposi == id_deposi &&
                x.IdDepto == id_depto);

            if (depto == null)
                return vRet;

            var region = await context.Regions.FirstOrDefaultAsync(r => r.IdRegion == id_region);
            var planta = await context.Planta.FirstOrDefaultAsync(p => p.IdRegion == id_region && p.IdPlanta == id_planta);
            var zona = await context.Zonas.FirstOrDefaultAsync(z => z.IdRegion == id_region && z.IdPlanta == id_planta && z.IdZona == id_zona);
            var deposito = await context.Depositos.FirstOrDefaultAsync(d => d.IdRegion == id_region && d.IdPlanta == id_planta && d.IdZona == id_zona && d.IdDeposi == id_deposi);

            vRet = new DeptoViewModel
            {
                id_region = depto.IdRegion,
                id_planta = depto.IdPlanta,
                id_zona = (short)depto.IdZona,
                id_deposi = depto.IdDeposi,
                id_depto = depto.IdDepto,
                descrip = depto.Descrip?.Trim(),
                responsable = depto.Responsable?.Trim(),
                nombre_region = region?.Nombre,
                nombre_planta = planta?.Nombre,
                nombre_zona = zona?.NombreZona,
                nombre_deposi = deposito?.Descrip
            };

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza un departamento. Simula las FK validando validando la jerarquía.
        /// </summary>
        /// <param name="model">Modelo de datos del Departamento</param>
        /// <returns>Verdadero si la operación es exitosa</returns>
        public async Task<bool> AgregarDepto(DeptoViewModel model)
        {
            // Validar existencia de dependencias
            bool existeRegion = await context.Regions.AnyAsync(r => r.IdRegion == model.id_region);
            if (!existeRegion) throw new Exception($"Restricción referencial: La región con Id {model.id_region} no existe.");

            bool existePlanta = await context.Planta.AnyAsync(p => p.IdRegion == model.id_region && p.IdPlanta == model.id_planta);
            if (!existePlanta) throw new Exception($"Restricción referencial: La planta con Id {model.id_planta} no existe en la región {model.id_region}.");

            bool existeZona = await context.Zonas.AnyAsync(z => z.IdRegion == model.id_region && z.IdPlanta == model.id_planta && z.IdZona == model.id_zona);
            if (!existeZona) throw new Exception($"Restricción referencial: La zona con Id {model.id_zona} no existe.");

            bool existeDeposi = await context.Depositos.AnyAsync(d => d.IdRegion == model.id_region && d.IdPlanta == model.id_planta && d.IdZona == model.id_zona && d.IdDeposi == model.id_deposi);
            if (!existeDeposi) throw new Exception($"Restricción referencial: El depósito con Id {model.id_deposi} no existe.");

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
                    throw new Exception("No se encontró el departamento");
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

                var vLastRow = await context.Deptos
                    .Where(x => x.IdRegion == model.id_region && x.IdPlanta == model.id_planta && x.IdZona == model.id_zona && x.IdDeposi == model.id_deposi)
                    .OrderByDescending(x => x.IdDepto)
                    .FirstOrDefaultAsync();

                depto.IdDepto = (short)((vLastRow != null ? vLastRow.IdDepto : 0) + 1);
            }

            var deptoExistente = await context.Deptos.FirstOrDefaultAsync(x =>
                x.Descrip.ToLower() == model.descrip.ToLower() &&
                x.IdRegion == model.id_region &&
                x.IdPlanta == model.id_planta &&
                x.IdZona == model.id_zona &&
                x.IdDeposi == model.id_deposi &&
                x.IdDepto != model.id_depto);

            if (deptoExistente != null)
                throw new Exception("Ya existe un departamento con la misma descripción en este depósito.");

            depto.Descrip = model.descrip;
            depto.Responsable = model.responsable;

            if (model.id_depto > 0)
            {
                context.Deptos.Entry(depto);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Deptos.Add(depto);
                await context.SaveChangesAsync();
            }

            return true;
        }
    }
}
