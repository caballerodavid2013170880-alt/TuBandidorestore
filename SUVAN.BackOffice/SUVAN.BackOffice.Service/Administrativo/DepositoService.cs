using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class DepositosService : IDepositoService
    {
        private readonly SuvanDbContext context;

        public DepositosService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene el listado de empresas desde la base de datos.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa.</param>
        /// <returns>Lista de empresas.</returns>
        public async Task<List<Deposito>> GetDepositos(int idEmpresa)
        {
            var depositos = await context.Depositos
                .Include(d => d.IdZonaNavigation)
                .Include(d => d.IdEmpresaNavigation)
                .Where(d => d.IdEmpresa == idEmpresa)
                .OrderBy(d => d.IdZonaNavigation.NombreZona)
                .ThenBy(d => d.NombreDeposito)
                .ToListAsync();

            return depositos!;
        }

        /// <summary>
        /// Obtiene el ViewModel para el dep�sito espec�fico.
        /// </summary>
        /// <param name="nombre">Nombre del dep�sito.</param>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        ///  <param name="id_region">Identificador de la regi�n.</param>
        ///  <param name="id_planta">Identificador de la planta.</param>
        ///  <param name="id_zona">Identificador de la zona.</param>
        /// <param name="id_deposito">Identificador del dep�sito.</param>
        /// <returns>ViewModel para el dep�sito espec�fico.</returns>
        public async Task<DepositoViewModel> GetDepositoViewModel(int idEmpresa, int idDeposito)
        {
            // Carga de regiones disponibles para la empresa
            var regiones = await context.Regions
                .Where(r => r.IdEmpresa == idEmpresa)
                .OrderBy(r => r.NombreRegion)
                .Select(r => new DepositoViewModel.CatalogItemViewModel
                {
                    Id = r.IdRegion,
                    Nombre = r.NombreRegion
                })
                .ToListAsync();

            var vRet = new DepositoViewModel
            {
                Regiones = regiones,
                IdEmpresa = idEmpresa,
                ActivoBool = true // Activo por defecto al crear
            };

            // Al Editar: carga datos del depósito y prellena los selectores
            if (idDeposito > 0)
            {
                var deposito = await context.Depositos
                    .FirstOrDefaultAsync(d => d.IdDeposito == idDeposito && d.IdEmpresa == idEmpresa);

                if (deposito == null)
                    throw new Exception("El depósito no pertenece a su empresa o no existe.");

                vRet.IdDeposito = deposito.IdDeposito;
                vRet.IdEmpresa = deposito.IdEmpresa;
                vRet.IdRegion = deposito.IdRegion;
                vRet.IdPlanta = deposito.IdPlanta;
                vRet.IdZona = deposito.IdZona;
                vRet.NombreDeposito = deposito.NombreDeposito;
                vRet.Direc = deposito.Direc;
                vRet.Ciudad = deposito.Ciudad;
                vRet.Respon = deposito.Respon;
                vRet.Tel = deposito.Tel;
                vRet.LocFor = deposito.LocFor;
                vRet.RPerson = deposito.RPerson;
                vRet.NomCorto = deposito.NomCorto;
                vRet.Rfc = deposito.Rfc;
                vRet.Cp = deposito.Cp;
                vRet.Activo = deposito.Activo ?? 0;

                vRet.Plantas = await context.Planta
                    .Where(p => p.IdEmpresa == idEmpresa && p.IdRegion == deposito.IdRegion)
                    .OrderBy(p => p.NombrePlanta)
                    .Select(p => new DepositoViewModel.CatalogItemViewModel
                    {
                        Id = p.IdPlanta,
                        Nombre = p.NombrePlanta
                    })
                    .ToListAsync();

                vRet.Zonas = await context.Zonas
                    .Where(z => z.IdEmpresa == idEmpresa && z.IdRegion == deposito.IdRegion && z.IdPlanta == deposito.IdPlanta)
                    .OrderBy(z => z.NombreZona)
                    .Select(z => new DepositoViewModel.CatalogItemViewModel
                    {
                        Id = z.IdZona,
                        Nombre = z.NombreZona
                    })
                    .ToListAsync();
            }

            return vRet;

        }
        /// <summary>
        /// Agrega o actualiza un dep�sito en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del dep�sito.</param>
        /// <returns>True si la operaci�n fue exitosa, de lo contrario, lanza una excepci�n.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarDeposito(DepositoViewModel model, int idEmpresa)
        {
            // Validar que la región pertenece a la empresa del usuario
            bool regionValida = await context.Regions.AnyAsync(r => r.IdRegion == model.IdRegion && r.IdEmpresa == idEmpresa);
            if (!regionValida) throw new Exception("La región seleccionada no pertenece a su empresa.");

            // Validar que la planta pertenece a la región y empresa
            bool plantaValida = await context.Planta.AnyAsync(p => p.IdPlanta == model.IdPlanta && p.IdRegion == model.IdRegion && p.IdEmpresa == idEmpresa);
            if (!plantaValida) throw new Exception("La planta seleccionada no pertenece a la región y empresa indicadas.");

            // Validar que la zona pertenece a la región, planta y empresa
            bool zonaValida = await context.Zonas.AnyAsync(z => z.IdZona == model.IdZona && z.IdPlanta == model.IdPlanta && z.IdRegion == model.IdRegion && z.IdEmpresa == idEmpresa);
            if (!zonaValida) throw new Exception("La zona seleccionada no pertenece a la planta, región y empresa indicadas.");

            Deposito deposito;
            if (model.IdDeposito > 0)
            {
                deposito = await context.Depositos.FirstOrDefaultAsync(d => d.IdDeposito == model.IdDeposito && d.IdEmpresa == idEmpresa);
                if (deposito == null) throw new Exception("El depósito no pertenece a su empresa o no existe.");
            }
            else
            {
                deposito = new Deposito();
                var lastId = await context.Depositos
                    .OrderByDescending(d => d.IdDeposito)
                    .Select(d => (int?)d.IdDeposito)
                    .FirstOrDefaultAsync();
                deposito.IdDeposito = (lastId ?? 0) + 1;
            }

            // Validar nombre duplicado en la misma zona
            bool nombreDuplicado = await context.Depositos.AnyAsync(d =>
                d.NombreDeposito!.Trim().ToLower() == model.NombreDeposito!.Trim().ToLower() &&
                d.IdZona == model.IdZona && d.IdEmpresa == idEmpresa &&
                d.IdDeposito != model.IdDeposito);

            if (nombreDuplicado) throw new Exception("Ya existe un depósito con el mismo nombre en esta zona.");

            deposito.IdEmpresa = idEmpresa;
            deposito.IdRegion = model.IdRegion;
            deposito.IdPlanta = model.IdPlanta;
            deposito.IdZona = model.IdZona;
            deposito.NombreDeposito = model.NombreDeposito;
            deposito.Direc = model.Direc;
            deposito.Ciudad = model.Ciudad;
            deposito.Respon = model.Respon;
            deposito.Tel = model.Tel;
            deposito.LocFor = model.LocFor?.Trim().ToUpper().Substring(0, 1);
            deposito.RPerson = model.RPerson;
            deposito.NomCorto = model.NomCorto;
            deposito.Rfc = model.Rfc;
            deposito.Cp = model.Cp;
            deposito.Activo = model.Activo;

            if (model.IdDeposito > 0)
            {
                await context.SaveChangesAsync();
            }
            else
            {
                context.Depositos.Add(deposito);
                await context.SaveChangesAsync();
            }

            return true;
                    }
                }
            }
