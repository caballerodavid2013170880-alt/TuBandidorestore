using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
    public class PlantaService : IPlantaService
    {
        private readonly SuvanDbContext context;

        public PlantaService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene el listado de empresas desde la base de datos.
        /// </summary>
        /// <param name="idemp">Identificador de la empresa.</param>
        /// <returns>Lista de Plantas.</returns>
        public async Task<List<Plantum>> GetPlantas(int id_emp)
        {
            var plantas = await context.Planta.Where(x => x.IdEmp == id_emp).ToListAsync();
            return plantas!;
        }
        /// <summary>
        /// Obtiene el ViewModel para la región específica.
        /// </summary>
        /// <param name="id_emp">Identificador de la empresa.</param>
        /// <param name="id_region">Identificador de la región.</param>
        /// <param name="id_planta"> Identificador de planta </param>
        /// <returns>ViewModel para la planta específica.</returns>
        public async Task<PlantaViewModel> GetPlantaViewModel(int id_emp, int id_region, int id_planta)
        {
            PlantaViewModel vRet = new PlantaViewModel();
            var planta = await context.Planta.FirstOrDefaultAsync(x => x.IdEmp == id_emp && x.IdRegion == id_region && x.IdPlanta == id_planta);

            if (planta == null)
                return vRet;
            else
            {
                var region = await context.Regions.FirstOrDefaultAsync(r => r.IdRegion == planta.IdRegion && r.IdEmpresa == planta.IdEmp);

                vRet = new PlantaViewModel
                {
                    id_emp = (short)planta.IdEmp,
                    id_region = (short)planta.IdRegion,
                    id_planta = planta.IdPlanta,
                    nombre = planta.Nombre?.Trim(),
                    libreria = planta.Libreria?.Trim(),
                    nombre_region = region?.Nombre
                };

            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza una planta en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la planta.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarPlanta(PlantaViewModel model)
        {
            // Validación de integridad referencial simulada 
            // Validar existencia de Empresa (solo si el modelo trae un IdEmp asignado)
            if (model.id_emp != null)
            {
                bool existeEmpresa = await context.Empresas.AnyAsync(e => e.Idempresa == model.id_emp);
                if (!existeEmpresa)
                    throw new Exception($"Restricción referencial: La empresa con Id {model.id_emp} no existe.");
            }
            // Validar existencia de Región
            bool existeRegion = await context.Regions.AnyAsync(r => r.IdRegion == model.id_region);
            if (!existeRegion)
                throw new Exception($"Restricción referencial: La región con Id {model.id_region} no existe.");

            Plantum planta;

            if (model.id_planta > 0)
            {
                // Búsqueda simplificada: Ignora el id_region del modelo en un nurevo registro, 280426
                // previene cualquier inyección desde el frontend al intentar cambiar la región.
                //planta = await context.Planta.FirstOrDefaultAsync(x => x.IdEmp == model.id_emp && x.IdPlanta == model.id_planta); 280426
                planta = await context.Planta.FirstOrDefaultAsync(x => x.IdEmp == (short?)model.id_emp && x.IdPlanta == model.id_planta);

                if (planta == null)
                    throw new Exception("No se encontró la planta");

                // Mantiene la región original en BD para no sobreescribir.
                model.id_region = (short)planta.IdRegion;
            }
            else
            {
                planta = new Plantum();

                // Conversión explícita a short? 280426
                planta.IdEmp = (short?)model.id_emp;

                planta.IdRegion = model.id_region;

                //var vLastRow = await context.Planta.OrderBy(x => x.IdPlanta).LastOrDefaultAsync(x => x.IdEmp == model.id_emp && x.IdRegion == model.id_region);
                var vLastRow = await context.Planta.OrderByDescending(x => x.IdPlanta).FirstOrDefaultAsync();

                planta.IdPlanta = (short)((vLastRow != null ? vLastRow.IdPlanta : 0) + 1);
            }
            // validate if exist one planta with the same name in the same empresa

            // var plantaExistente = await context.Planta.FirstOrDefaultAsync(x => x.Nombre!.ToLower() == model.nombre!.ToLower() 280426
            // && x.IdEmp == model.id_emp && x.IdPlanta != model.id_planta && x.IdRegion == model.id_region);   280426
            var plantaExistente = await context.Planta.FirstOrDefaultAsync(x => x.Nombre!.ToLower() == model.nombre!.ToLower()
            && x.IdEmp == (short?)model.id_emp && x.IdPlanta != model.id_planta && x.IdRegion == model.id_region);

            if (plantaExistente is not null)
                throw new Exception("Ya existe una planta con el mismo nombre en esta región.");

            planta.Nombre = model.nombre;
            planta.Libreria = model.libreria;

            if (model.id_planta > 0)
            {
                context.Planta.Entry(planta);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Planta.Add(planta);
                await context.SaveChangesAsync();
            }
            return true;
        }
    }
}