using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class DeptoService : IDeptoService
    {
        private readonly SuvanDbContext context;
        public DeptoService(SuvanDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Obtiene el listado de departamentos de la empresa indicada,
        /// incluyendo la navegaci�n a <see cref="Deposito"/> para mostrar
        /// el nombre del dep�sito en la tabla de la vista <c>Deptos.cshtml</c>.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.Act�a como filtro de seguridad sobre la consulta.
        /// </param>
        /// <returns>
        /// Lista de <see cref="Depto"/> con <c>IdDepositoNavigation</c> cargada
        /// mediante eager loading, orden por nombre de dep�sito y luego por nombre de departamento.
        /// </returns>
        public async Task<List<Depto>> GetDepto(int idEmpresa)
        {
            var deptos = await context.Deptos
                .Include(d => d.Id) // Para mostrar NombreDeposito en la tabla
                .Include(d => d.IdEmpresaNavigation)
                .Where(d => d.IdEmpresa == idEmpresa)
                .OrderBy(d => d.IdDepositoNavigation.NombreDeposito)
                .ThenBy(d => d.NombreDepto)
                .ToListAsync();
            return deptos;
        }
        /// <summary>
        /// Construye el ViewModel para el formulario de alta o edici�n de un departamento.
        /// Al Agregar solo carga las regiones. Al Editar pre-carga los cuatro selectores con los datos del departamento respetando la jerarqu�a de seguridad.
        /// </summary>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado.Determina qu� regiones, plantas, zonas y dep�sitos se muestran.
        /// </param>
        /// <param name="idDepto">
        /// Identificador del departamento a editar.
        /// Pasar <c>0</c> para modo alta (ViewModel vac�o con solo regiones).
        /// </param>
        /// <returns>
        /// <see cref="DeptoViewModel"/> poblado con las listas y datos necesarios.
        /// </returns>
        /// <exception cref="Exception">
        /// Si <paramref name="idDepto"/> es mayor a 0 y el departamento no existe o no pertenece a la empresa del usuario.
        /// </exception>
        public async Task<DeptoViewModel> GetDeptoViewModel(int idEmpresa, int idDepto)
        {
            // Carga de regiones disponibles para la empresa (siempre necesario)
            var regiones = await context.Regions
                .Where(r => r.IdEmpresa == idEmpresa)
                .OrderBy(r => r.NombreRegion)
                .Select(r => new DeptoViewModel.CatalogItemViewModel
                {
                    Id = r.IdRegion,
                    Nombre = r.NombreRegion
                })
                .ToListAsync();
            
            var vRet = new DeptoViewModel
            {
                Regiones = regiones,
                IdEmpresa = idEmpresa,
                ActivoBool = true  // Activo por defecto al crear
            };
            // Al Editar: carga datos del departamento y pre-llena los cuatro selectores
            if (idDepto > 0)
            {
                // Validaci�n de seguridad: el departamento debe pertenecer a la empresa del usuario
                var depto = await context.Deptos
                    .FirstOrDefaultAsync(d => d.IdDepto == idDepto && d.IdEmpresa == idEmpresa);
                if (depto == null)
                    throw new Exception("El departamento no pertenece a su empresa o no existe.");
                // Asignaci�n de campos del departamento al ViewModel
                vRet.IdDepto = depto.IdDepto;
                vRet.IdEmpresa = depto.IdEmpresa;
                vRet.IdRegion = depto.IdRegion;
                vRet.IdPlanta = depto.IdPlanta;
                vRet.IdZona = depto.IdZona;
                vRet.IdDeposito = depto.IdDeposito;
                vRet.NombreDepto = depto.NombreDepto;
                vRet.Responsable = depto.Responsable;
                vRet.Activo = depto.Activo ?? 0;
                // Pre-carga plantas de la regi�n guardada (jerarqu�a empresa + regi�n)
                vRet.Plantas = await context.Planta
                    .Where(p => p.IdEmpresa == idEmpresa && p.IdRegion == depto.IdRegion)
                    .OrderBy(p => p.NombrePlanta)
                    .Select(p => new DeptoViewModel.CatalogItemViewModel
                    {
                        Id = p.IdPlanta,
                        Nombre = p.NombrePlanta
                    })
                    .ToListAsync();
                // Pre-carga zonas de la planta guardada (jerarqu�a empresa + regi�n + planta)
                vRet.Zonas = await context.Zonas
                    .Where(z => z.IdEmpresa == idEmpresa
                             && z.IdRegion == depto.IdRegion
                             && z.IdPlanta == depto.IdPlanta)
                    .OrderBy(z => z.NombreZona)
                    .Select(z => new DeptoViewModel.CatalogItemViewModel
                    {
                        Id = z.IdZona,
                        Nombre = z.NombreZona
                    })
                    .ToListAsync();
                // Pre-carga dep�sitos de la zona guardada (jerarqu�a empresa + regi�n + planta + zona)
                vRet.Depositos = await context.Depositos
                    .Where(d => d.IdEmpresa == idEmpresa
                             && d.IdRegion == depto.IdRegion
                             && d.IdPlanta == depto.IdPlanta
                             && d.IdZona == depto.IdZona)
                    .OrderBy(d => d.NombreDeposito)
                    .Select(d => new DeptoViewModel.CatalogItemViewModel
                    {
                        Id = d.IdDeposito,
                        Nombre = d.NombreDeposito
                    })
                    .ToListAsync();
            }
            return vRet;
        }
        /// <summary>
        /// Agrega o actualiza un departamento en la base de datos.Valida la jerarqu�a completa: Empresa ? Regi�n ? Planta ? Zona ? Dep�sito.
        /// Al agregar genera el <c>IdDepto</c> como MAX global + 1.
        /// </summary>
        /// <param name="model">ViewModel con los datos capturados en el formulario.</param>
        /// <param name="idEmpresa">
        /// Identificador de la empresa del usuario autenticado. Se utiliza para validar cada nivel jer�rquico y sobrescribir la empresa en la entidad.
        /// </param>
        /// <returns><c>true</c> si la operaci�n fue exitosa.</returns>
        /// <exception cref="Exception">Si alguna validaci�n de seguridad o de negocio falla.</exception>
        public async Task<bool> AgregarDepto(DeptoViewModel model, int idEmpresa)
        {
            // 1. Validar que la región pertenece a la empresa del usuario
            bool regionValida = await context.Regions
                .AnyAsync(r => r.IdRegion == model.IdRegion && r.IdEmpresa == idEmpresa);
            if (!regionValida)
                throw new Exception("La regi�n seleccionada no pertenece a su empresa.");
            // 2. Validar que la planta pertenece a la región y empresa
            bool plantaValida = await context.Planta
                .AnyAsync(p => p.IdPlanta == model.IdPlanta
                            && p.IdRegion == model.IdRegion
                            && p.IdEmpresa == idEmpresa);
            if (!plantaValida)
                throw new Exception("La planta seleccionada no pertenece a la regi�n y empresa indicadas.");
            // 3. Validar que la zona pertenece a la región, planta y empresa
            bool zonaValida = await context.Zonas
                .AnyAsync(z => z.IdZona == model.IdZona
                            && z.IdRegion == model.IdRegion
                            && z.IdPlanta == model.IdPlanta
                            && z.IdEmpresa == idEmpresa);
            if (!zonaValida)
                throw new Exception("La zona seleccionada no pertenece a la planta, regi�n y empresa indicadas.");
            // 4. Validar que el dep�sito pertenece a la región, planta, zona y empresa
            bool depositoValido = await context.Depositos
                .AnyAsync(d => d.IdDeposito == model.IdDeposito
                            && d.IdRegion == model.IdRegion
                            && d.IdPlanta == model.IdPlanta
                            && d.IdZona == model.IdZona
                            && d.IdEmpresa == idEmpresa);
            if (!depositoValido)
                throw new Exception("El deposito seleccionado no pertenece a la zona, planta, regi�n y empresa indicadas.");
            Depto depto;
            if (model.IdDepto > 0)
            {
                // Al Editar: valida que el departamento pertenece a la empresa del usuario
                depto = await context.Deptos
                    .FirstOrDefaultAsync(d => d.IdDepto == model.IdDepto && d.IdEmpresa == idEmpresa);
                if (depto == null)
                    throw new Exception("El departamento no pertenece a su empresa o no existe.");
            }
            else
            {
                // Al Agregar: crea nueva instancia y genera el siguiente IdDepto (global MAX + 1)
                depto = new Depto();
                var lastId = await context.Deptos
                    .OrderByDescending(d => d.IdDepto)
                    .Select(d => (int?)d.IdDepto)
                    .FirstOrDefaultAsync();
                depto.IdDepto = (lastId ?? 0) + 1;
            }
            // 5. Validar nombre duplicado en el mismo depósito y empresa (excluyendo el registro actual en edición)
            bool nombreDuplicado = await context.Deptos
                .AnyAsync(d =>
                    d.NombreDepto!.Trim().ToLower() == model.NombreDepto!.Trim().ToLower() &&
                    d.IdDeposito == model.IdDeposito &&
                    d.IdEmpresa == idEmpresa &&
                    d.IdDepto != model.IdDepto);
            if (nombreDuplicado)
                throw new Exception("Ya existe un Departamento con el mismo nombre en este Dep�sito.");
            // Asignaci�n de valores a la entidad
            depto.IdEmpresa = idEmpresa;
            depto.IdRegion = model.IdRegion;
            depto.IdPlanta = model.IdPlanta;
            depto.IdZona = model.IdZona;
            depto.IdDeposito = model.IdDeposito;
            depto.NombreDepto = model.NombreDepto;
            depto.Responsable = model.Responsable!;
            depto.Activo = model.Activo;
            if (model.IdDepto > 0)
            {
                // Actualizar: la entidad por EF al hacer FirstOrDefaultAsync
                await context.SaveChangesAsync();
            }
            else
            {
                // Insertar nuevo registro
                context.Deptos.Add(depto);
                await context.SaveChangesAsync();
            }
            return true;
        }

        // ------------------------------------------------------------------
        //  Endpoints de cascada para los selectores AJAX
        // ------------------------------------------------------------------
        /// <summary>
        /// Obtiene plantas filtradas por empresa y regi�n.Consumido como endpoint AJAX para la cascada Regi�n ? Planta.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa del usuario autenticado.</param>
        /// <param name="idRegion">Identificador de la regi�n seleccionada.</param>
        /// <returns>
        /// Lista de <see cref="DeptoViewModel.CatalogItemViewModel"/> ordenada por nombre.
        /// </returns>
        public async Task<List<DeptoViewModel.CatalogItemViewModel>> GetPlantasPorRegion(int idEmpresa, int idRegion)
        {
            return await context.Planta
                .Where(p => p.IdEmpresa == idEmpresa && p.IdRegion == idRegion)
                .OrderBy(p => p.NombrePlanta)
                .Select(p => new DeptoViewModel.CatalogItemViewModel
                {
                    Id = p.IdPlanta,
                    Nombre = p.NombrePlanta
                })
                .ToListAsync();
        }
        /// <summary>
        /// Obtiene zonas filtradas por empresa, regi�n y planta.Consumido como endpoint AJAX para la cascada Planta ? Zona.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa del usuario autenticado.</param>
        /// <param name="idRegion">Identificador de la regi�n seleccionada.</param>
        /// <param name="idPlanta">Identificador de la planta seleccionada.</param>
        /// <returns>
        /// Lista de <see cref="DeptoViewModel.CatalogItemViewModel"/> ordenada por nombre.
        /// </returns>
        public async Task<List<DeptoViewModel.CatalogItemViewModel>> GetZonasPorPlanta(int idEmpresa, int idRegion, int idPlanta)
        {
            return await context.Zonas
                .Where(z => z.IdEmpresa == idEmpresa
                         && z.IdRegion == idRegion
                         && z.IdPlanta == idPlanta)
                .OrderBy(z => z.NombreZona)
                .Select(z => new DeptoViewModel.CatalogItemViewModel
                {
                    Id = z.IdZona,
                    Nombre = z.NombreZona
                })
                .ToListAsync();
        }
        /// <summary>
        /// Obtiene dep�sitos filtrados por empresa, regi�n, planta y zona.Consumido como endpoint AJAX para la cascada Zona ? Dep�sito.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa del usuario autenticado.</param>
        /// <param name="idRegion">Identificador de la regi�n seleccionada.</param>
        /// <param name="idPlanta">Identificador de la planta seleccionada.</param>
        /// <param name="idZona">Identificador de la zona seleccionada.</param>
        /// <returns>
        /// Lista de <see cref="DeptoViewModel.CatalogItemViewModel"/> ordenada por nombre.
        /// </returns>
        public async Task<List<DeptoViewModel.CatalogItemViewModel>> GetDepositosPorZona(int idEmpresa, int idRegion, int idPlanta, int idZona)
        {
            return await context.Depositos
                .Where(d => d.IdEmpresa == idEmpresa
                         && d.IdRegion == idRegion
                         && d.IdPlanta == idPlanta
                         && d.IdZona == idZona)
                .OrderBy(d => d.NombreDeposito)
                .Select(d => new DeptoViewModel.CatalogItemViewModel
                {
                    Id = d.IdDeposito,
                    Nombre = d.NombreDeposito
                })
                .ToListAsync();
        }
    }
}









