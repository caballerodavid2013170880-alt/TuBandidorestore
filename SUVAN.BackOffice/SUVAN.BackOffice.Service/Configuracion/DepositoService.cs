using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
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
        /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <returns>Lista de empresas.</returns>
        public async Task<List<Deposito>> GetDepositos(int id_empresa)
        {
            var depositos = await context.Depositos
                .Where(x => x.IdEmpresa == id_empresa && x.Activo.GetValueOrDefault() == 1)
                .ToListAsync();

            return depositos!;
        }


        ////Filtrado para no mostrar los borrados
        //public async Task<List<Deposito>> GetDepositos(int id_empresa)
        //{
        //    return await context.Depositos
        //        .Where(x => x.IdEmpresa == id_empresa && x.Activo == 1)
        //        .ToListAsync();
        //}

        /// <summary>
        /// Obtiene el ViewModel para el depósito específico.
        /// </summary>
        /// <param name="nombre">Nombre del depósito.</param>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        ///  <param name="id_region">Identificador de la región.</param>
        ///  <param name="id_planta">Identificador de la planta.</param>
        ///  <param name="id_zona">Identificador de la zona.</param>
        /// <param name="id_deposito">Identificador del depósito.</param>
        /// <returns>ViewModel para el depósito específico.</returns>
        public async Task<DepositoViewModel> GetDepositoViewModel(int id_empresa, int id_deposito)
        {
            DepositoViewModel vRet = new DepositoViewModel();
            var deposito = await context.Depositos.FirstOrDefaultAsync(x => x.IdEmpresa == id_empresa && x.IdDeposito == id_deposito);

            if (deposito == null)
                return vRet;
            else
            {
                vRet = new DepositoViewModel
                {
                    IdEmpresa = deposito.IdEmpresa,
                    IdRegion = deposito.IdRegion,
                    IdPlanta = deposito.IdPlanta,
                    IdZona = deposito.IdZona,
                    IdDeposito = deposito.IdDeposito,
                    NombreDeposito = deposito.NombreDeposito,
                    Direc = deposito.Direc,
                    Ciudad = deposito.Ciudad,
                    Respon = deposito.Respon,
                    Tel = deposito.Tel,
                    LocFor = deposito.LocFor,
                    RPerson = deposito.RPerson,
                    NomCorto = deposito.NomCorto,
                    Rfc = deposito.Rfc,
                    Cp = deposito.Cp
                };
            }

            return vRet;

        }
        /// <summary>
        /// Agrega o actualiza un depósito en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del depósito.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarDeposito(DepositoViewModel model)
        {
            Deposito deposito;


            if (model.IdDeposito > 0)
            {
                deposito = await context.Depositos.FirstOrDefaultAsync(x => x.IdEmpresa == model.IdEmpresa && x.IdDeposito == model.IdDeposito);

                if (deposito == null)
                    throw new Exception("No se encontro el deposito. ");
            }
            else
            {
                deposito = new Deposito();
                deposito.IdEmpresa = model.IdEmpresa;

                //// se agrega el identity manualmente que no esta en la bd
                //var vLastRow = await context.Depositos.OrderBy(x => x.IdEmpresa).LastOrDefaultAsync(x => x.IdEmpresa == model.id_empresa);
                //deposito.IdDeposi = (short)((vLastRow != null ? vLastRow.IdDeposi : 0) + 1);

                // se agrega el identity manualmente que no esta en la bd
                //var vLastRow = await context.Depositos.OrderByDescending(x => x.IdDeposito).FirstOrDefaultAsync();
                //deposito.IdDeposito = (short)((vLastRow != null ? vLastRow.IdDeposito : 0) + 1);
            }

            // validate if exist one deposit with the same name in the same empresa
            var depositoExistente = await context.Depositos.FirstOrDefaultAsync(x => x.NombreDeposito!.ToLower() == model.NombreDeposito!.ToLower()
            && x.IdEmpresa == model.IdEmpresa 
            && x.IdDeposito != model.IdDeposito
            && x.Activo.GetValueOrDefault() == 1);//validacion de los que estan activos

            if (depositoExistente is not null)
                throw new Exception("Ya existe un depósito con el mismo nombre en la empresa");


            deposito.NombreDeposito = model.NombreDeposito;
            deposito.IdRegion = model.IdRegion;
            deposito.IdPlanta = model.IdPlanta;
            deposito.IdZona = model.IdZona;
            deposito.Direc = model.Direc;
            deposito.Ciudad = model.Ciudad;
            deposito.Respon = model.Respon;
            deposito.Tel = model.Tel;
            deposito.LocFor = model.LocFor;
            deposito.RPerson = model.RPerson;
            deposito.NomCorto = model.NomCorto;
            deposito.Rfc = model.Rfc;
            deposito.Cp = model.Cp;
            //forzar el valor de locfor a ser la primera letra en mayuscula
            deposito.LocFor = model.LocFor?.Trim().ToUpper().Substring(0,1);

            //asignacion del borrado 
            //se traduce el bool del ViewModel al ulong del Entity
            deposito.Activo = model.Activo ? (ulong)1 : (ulong)0;

            if (model.IdDeposito > 0)
            {
                //Notifica al contexto que este objeto ya fue modificado 
                context.Entry(deposito).State=EntityState.Modified;
                await context.SaveChangesAsync();
            }
            else
            {
                context.Depositos.Add(deposito);
                await context.SaveChangesAsync();

            }
            return true;
        }



        public async Task<List<RegionModel>> GetRegions(int id_empresa)
        {
            return await context.Regions
                .Where(x => x.IdEmpresa == id_empresa)
                .Select(x => new RegionModel {
                    Id = x.IdRegion,
                    Nombre = x.NombreRegion
            }).ToListAsync();
        }

        public async Task<List<RegionModel>> GetPlantas(int id_empresa)
        {
            return await context.Planta
                .Where(x => x.IdEmpresa == id_empresa)
                .Select(x => new RegionModel {
                    Id = x.IdPlanta,
                    Nombre = x.NombrePlanta
            }).ToListAsync();
        }

        public async Task<List<RegionModel>> GetZonas(int id_empresa)
        {
            return await context.Zonas
                .Where(x => x.IdEmpresa == id_empresa)
                .Select(x => new RegionModel {
                    Id = x.IdZona,
                    Nombre = x.NombreZona
                }).ToListAsync();            
        }
              
        //NUEVOS METODOS PARA COMBOS EN CASCADA

        /// <summary>
        /// Obtiene las plantas disponibles para una región específica.
        /// <summary>
        public async Task<List<RegionModel>> GetPlantasByRegion(int id_empresa, int id_region)
        {
            return await context.Planta
                .Where(x => x.IdEmpresa == id_empresa && x.IdRegion == (short)id_region)
                .Select(x => new RegionModel
                {
                    Id = x.IdPlanta,
                    Nombre = x.NombrePlanta
                }).ToListAsync();
        }

        /// <summary>
        /// Obtiene las zonas disponibles para una planta específica.
        /// <summary>
        public async Task<List<RegionModel>> GetZonasByPlanta(int id_empresa, int id_planta)
        {
            return await context.Zonas
                .Where(x => x.IdEmpresa == id_empresa && x.IdPlanta == id_planta)
                .Select(x => new RegionModel
                {
                    Id = x.IdZona,
                    Nombre = x.NombreZona
                }).ToListAsync();
        }
    }
}
