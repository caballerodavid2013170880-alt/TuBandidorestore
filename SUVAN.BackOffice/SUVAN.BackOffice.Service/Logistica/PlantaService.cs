using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class PlantaService : IPlantaService
    {
        private readonly SuvanDbContext context;

        public PlantaService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Identificador de la empresa del usuario autenticado.
        /// Actúa como filtro de seguridad sobre la consulta.
        /// </param>
        /// <param name="idEmpresa">
        /// Identificador de la empresa.
        /// </param>
        /// </summary>
        /// <returns>
        /// Lista de "Plantas" con la Región cargada mediante eager loading.
        /// </returns>
        public async Task<List<Plantum>> GetPlantas(int idEmpresa)
        {
            var plantas = await context.Planta
                .Include(p => p.Id)                    // Navegación confirmada: "Id" → Region
                .Include(p => p.IdEmpresaNavigation)   // Empresa padre
                .Where(p => p.IdEmpresa == idEmpresa)
                .OrderBy(p => p.Id.NombreRegion)
                .ThenBy(p => p.NombrePlanta)
                .ToListAsync();
            return plantas;
        }

        // Metodo entidades relacionadas POST 2do Scaffold 260526

        /// <summary>
        /// Construye el ViewModel para el formulario de alta o edición de una planta.
        /// Regiones del selector se filtran por empresa del usuario,respetando la jerarquía empresa → región.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.Regiones contenidas en el selector.
        /// </param>
        /// <param name="idPlanta">
        /// Identificador de la planta a editar. Pasar 0 para modo alta.
        /// </param>
        /// <returns>
        /// <see cref="PlantaViewModel"/> con las regiones disponibles y,si aplica, los datos de la planta existente.
        /// </returns>
        /// <exception cref="Exception">
        /// Si <paramref name="idPlanta"/> es mayor a 0 y la planta no pertenece a la empresa del usuario o no existe.
        /// </exception>
        public async Task<PlantaViewModel> GetPlantaViewModel(int idEmpresa, int idPlanta)
        {
            // Cargar las regiones disponibles para la empresa del usuario (jerarquía)
            var regiones = await context.Regions
                .Where(r => r.IdEmpresa == idEmpresa)
                .OrderBy(r => r.NombreRegion)
                .Select(r => new PlantaViewModel.RegionItemViewModel
                {
                    IdRegion = r.IdRegion,
                    Nombre = r.NombreRegion
                })
                .ToListAsync();
            PlantaViewModel vRet = new PlantaViewModel
            {
                Regiones = regiones,
                IdEmpresa = idEmpresa,
                ActivoBool = true    // Por defecto activo al crear
            };
            // Modo edición: cargar datos de la planta existente
            if (idPlanta > 0)
            {
                // Validación de seguridad: la planta debe pertenecer a la empresa del usuario
                var planta = await context.Planta
                    .FirstOrDefaultAsync(p => p.IdPlanta == idPlanta && p.IdEmpresa == idEmpresa);
                if (planta == null)
                    throw new Exception("La planta no pertenece a su empresa o no existe.");
                vRet.IdPlanta = planta.IdPlanta;
                vRet.IdRegion = planta.IdRegion;
                vRet.IdEmpresa = planta.IdEmpresa;
                vRet.NombrePlanta = planta.NombrePlanta;
                vRet.Libreria = planta.Libreria;
                vRet.Activo = planta.Activo ?? 0;
            }
            return vRet;
        }


        // Metodo entidades relacionadas POST 2do Scaffold 260526
        /// <summary>
        /// Agrega o actualiza una planta en la base de datos.
        /// Realiza las siguientes validaciones antes de persistir:
        /// <list type="bullet">
        ///   <item>La región seleccionada pertenece a la empresa del usuario.</item>
        ///   <item>En edición, la planta existe y pertenece a la empresa del usuario.</item>
        ///   <item>No existe una planta con el mismo nombre en la misma región y empresa.</item>
        /// </list>
        /// </summary>
        /// <param name="model">ViewModel con los datos capturados en el formulario.</param>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.Valida la región y asigna la empresa en la entidad.
        /// </param>
        /// <returns><c>true</c> si la operación fue exitosa.</returns>
        /// <exception cref="Exception">
        /// Si alguna validación de seguridad o de negocio falla.
        /// </exception>
        public async Task<bool> AgregarPlanta(PlantaViewModel model, int idEmpresa)
        {
            // Validación de seguridad: la región pertenece a la empresa del usuario
            bool regionValida = await context.Regions
                .AnyAsync(r => r.IdRegion == model.IdRegion && r.IdEmpresa == idEmpresa);
            if (!regionValida)
                throw new Exception("La región seleccionada no pertenece a su empresa.");
            Plantum planta;
            if (model.IdPlanta > 0)
            {
                // Modo edición — validar que la planta pertenece a la empresa del usuario
                planta = await context.Planta
                    .FirstOrDefaultAsync(p => p.IdPlanta == model.IdPlanta && p.IdEmpresa == idEmpresa);
                if (planta == null)
                    throw new Exception("La planta no pertenece a su empresa o no existe.");
            }
            else
            {
                // Modo alta — crear nueva instancia
                planta = new Plantum();
            }
            // Validar nombre duplicado en la misma región y empresa
            bool nombreDuplicado = await context.Planta
                .AnyAsync(p =>
                    p.NombrePlanta!.Trim().ToLower() == model.NombrePlanta!.Trim().ToLower() &&
                    p.IdEmpresa == idEmpresa &&
                    p.IdRegion == model.IdRegion &&
                    p.IdPlanta != model.IdPlanta);
            if (nombreDuplicado)
                throw new Exception("Ya existe una Planta con el mismo nombre en esta Región.");
            // Asignar valores a la entidad
            planta.NombrePlanta = model.NombrePlanta;
            planta.Libreria = model.Libreria;
            planta.Activo = model.Activo;
            planta.IdEmpresa = idEmpresa;
            planta.IdRegion = model.IdRegion;
            if (model.IdPlanta > 0)
            {
                // Actualizar registro existente
                context.Planta.Entry(planta);
                await context.SaveChangesAsync();
            }
            else
            {
                // Insertar nuevo registro
                context.Planta.Add(planta);
                await context.SaveChangesAsync();
            }
            return true;
        }
        // Metodo entidades relacionadas POST 2do Scaffold 260526
    }
}